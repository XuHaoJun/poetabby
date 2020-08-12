using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using NLua;
using System.Linq;

namespace RCB.JavaScript.Models.Utils
{
  public class PobUtils
  {
    private static System.IO.MemoryStream GenerateStreamFromString(string value)
    {
      return new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(value ?? ""));
    }

    static public string XmlToCode(string xmlStr)
    {
      byte[] bytes;
      using (var outputStream = new System.IO.MemoryStream())
      {
        using (zlib.ZOutputStream compressionStream = new zlib.ZOutputStream(outputStream, zlib.zlibConst.Z_DEFAULT_COMPRESSION))
        {
          byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(xmlStr);
          compressionStream.Write(byteArray, 0, byteArray.Length);
          compressionStream.Flush();
        }
        bytes = outputStream.ToArray();
      }
      return System.Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_");
    }

    static public string CodeToXml(string _code)
    {
      string code = _code.Replace("-", "+").Replace("_", "/");
      byte[] inData = System.Convert.FromBase64String(code);
      byte[] outData;
      using (MemoryStream outMemoryStream = new MemoryStream())
      {
        using (zlib.ZOutputStream outZStream = new zlib.ZOutputStream(outMemoryStream))
        {
          outZStream.Write(inData, 0, inData.Length);
          outZStream.Flush();
        }
        outData = outMemoryStream.ToArray();
      }
      return System.Text.Encoding.UTF8.GetString(outData);
    }

    static public PathOfBuilding XmlToPob(string xmlStr)
    {
      XmlSerializer serializer = new XmlSerializer(typeof(PathOfBuilding));
      PathOfBuilding pob;
      using (var ms = GenerateStreamFromString(xmlStr))
      using (var xr = XmlReader.Create(ms))
      {
        xr.Read();
        pob = (PathOfBuilding)serializer.Deserialize(xr);
      }
      return pob;
    }

    static private void runLuaLibLinkScript(string workingDirectory, int version = 7)
    {
      Process process = new Process();
      ProcessStartInfo start = new ProcessStartInfo();

      var cmd = $"ln -s libreadline.so.{version} libreadline.so.6";
      var escapedArgs = cmd.Replace("\"", "\\\"");

      start.WorkingDirectory = workingDirectory;
      start.FileName = "/bin/bash";
      start.Arguments = $"-c \"{escapedArgs}\"";
      start.CreateNoWindow = true;
      start.RedirectStandardOutput = true;
      start.RedirectStandardError = true;
      start.UseShellExecute = false;

      process.StartInfo = start;
      process.Start();

      String error = process.StandardError.ReadToEnd();
      String output = process.StandardOutput.ReadToEnd();

    }

    static public string GetPobDirPath()
    {
      string pobDirPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtraFiles", "pob");
      return pobDirPath;
    }

    static private Lua _LuaState = null;
    static private int _LuaStateNumCalls = 0;
    static private int _LuaStateLimitNumCalls = 10;

    static public Lua CreateLuaState()
    {
      Lua state = new Lua();
      string pobDirPath = GetPobDirPath();
      state["__pobDirPath__"] = pobDirPath;
      state.DoString("package.path = __pobDirPath__ .. '/?.lua;' .. package.path");
      state.DoString(@"
          local Path = require('path')
          _oldLoadfile = loadfile
          function loadfile(path)
            if Path.isabs(path) then
              return _oldLoadfile(path)
            else
              modifiedPath = __pobDirPath__ .. ""/"" .. path
              -- print(""loadfile: "" .. modifiedPath)
              return _oldLoadfile(modifiedPath)
            end
          end
          _oldDofile = dofile
          function dofile(path)
            if Path.isabs(path) then
              return _oldDofile(path)
            else
              modifiedPath = __pobDirPath__ .. ""/"" .. path
              -- print(""dofile: "" .. modifiedPath)
              return _oldDofile(modifiedPath)
            end
          end
          _oldIo = io
          local function open(path, mode)
            if Path.isabs(path) then
              return _oldIo.open(path)
            else
              if string.match(path, '%.png') or string.match(path, '%.jpg') then
                return nil
              else
                modifiedPath = __pobDirPath__ .. ""/"" .. path
                -- print(""io.open: "" .. modifiedPath)
                return _oldIo.open(modifiedPath, mode)
              end
            end
          end
          local function lines(path)
            if Path.isabs(path) then
              return _oldIo.lines(path)
            else
              modifiedPath = __pobDirPath__ .. '/' .. path
              -- print(""io.lines: "" .. modifiedPath)
              return _oldIo.lines(modifiedPath)
            end
          end
          io = { open=open, lines=lines, read=_oldIo.read, write=_oldIo.write, close=_oldIo.close, stderr=_oldIo.stderr }
      ");
      state.DoFile(Path.Combine(pobDirPath, "HeadlessWrapper.lua"));
      return state;
    }

    static public void DisposeLuaState()
    {
      if (_LuaState != null)
      {
        _LuaState.DoString(@"
          mainObject = nil
          build = nil
          collectgarbage('collect')
          ");
        _LuaState.Dispose();
        _LuaState = null;
      }
    }

    private class MyString
    {
      private string _value;
      public MyString(string value)
      {
        _value = value;
      }

      public string Value
      {
        get
        {
          return _value;
        }
        private set
        {
          _value = value;
        }
      }

    }
    private class JsonString : MyString
    {
      public JsonString(string value) : base(value)
      {
      }
    }


    private class XmlString : MyString
    {
      public XmlString(string value) : base(value)
      {
      }
    }

    static private object[] _GetBuildXmlHelper(Lua state, JsonString passivesJson, JsonString itemsJson)
    {
      using (var func = state.GetFunction("getBuildXmlByJsons"))
      {
        return func.Call(passivesJson.Value, itemsJson.Value);
      }
    }

    static private object[] _GetBuildXmlHelper(Lua state, XmlString xml)
    {
      using (var func = state.GetFunction("getBuildXmlByXml"))
      {
        return func.Call(xml.Value);
      }
    }

    static private string _GetBuildXml(MyString arg1, MyString arg2 = null)
    {
      string pobDirPath = GetPobDirPath();
      string pobLuaBinPath = GetPobLuaBinPath();
      if (!Directory.Exists(pobDirPath) || !File.Exists(pobLuaBinPath))
      {
        return null;
      }
      _LuaState = _LuaState == null ? CreateLuaState() : _LuaState;
      Lua state = _LuaState;
      object[] res;
      try
      {
        res = arg1 switch
        {
          JsonString x1 => arg2 switch
          {
            JsonString x2 => _GetBuildXmlHelper(state, x1, x2),
            _ => null
          },
          XmlString x1 => _GetBuildXmlHelper(state, x1),
          _ => null
        };
      }
      catch (NLua.Exceptions.LuaScriptException e)
      {
        // TODO
        // Log exception
        DisposeLuaState();
        res = null;
      }
      if (res != null)
      {
        string xml = new String((string)res.FirstOrDefault());
        foreach (var d in res.OfType<IDisposable>())
        {
          d.Dispose();
        }
        state.DoString("toListMode()");
        state.DoString("collectgarbage('collect')");
        // TODO
        // pob possible memory leak
        // https://github.com/Openarl/PathOfBuilding/issues/1372
        // if (_LuaStateNumCalls >= _LuaStateLimitNumCalls)
        // {
        //   _LuaStateNumCalls = 0;
        //   DisposeLuaState();
        // }
        // else
        // {
        //   _LuaStateNumCalls += 1;
        // }
        return xml;
      }
      else
      {
        return null;
      }
    }

    static public string GetBuildXmlByXml(string xmlString)
    {
      return _GetBuildXml(new XmlString(xmlString));
    }


    static public string GetBuildXmlByJsons(string passivesJson, string itemsJson)
    {
      return _GetBuildXml(new JsonString(passivesJson), new JsonString(itemsJson));
    }

    static public string Foo()
    {
      using (Neo.IronLua.Lua lua = new Neo.IronLua.Lua()) // Create the Lua script engine
      {
        dynamic env = lua.CreateEnvironment();
        string pobDirPath = GetPobDirPath();
        env["__pobDirPath__"] = pobDirPath;
        env["package"].path = Path.Combine(pobDirPath, "/?.lua");
        env.dochunk("dofile(__pobDirPath__ .. HeadlessWrapper.lua')", "test.lua");
        return "";
      }
    }

    static public string GetPobLuaBinPath()
    {
      bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                                     .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
      string luabinName = isWindows ? "lua52.exe" : "lua52";
      string pobDirPath = GetPobDirPath();
      string pobLuaBinPath = Path.Combine(pobDirPath, luabinName);
      return pobLuaBinPath;
    }

    static public string GetBuildXml2(string passivesJson, string itemsJson)
    {
      string pobDirPath = GetPobDirPath();
      string pobLuaBinPath = GetPobLuaBinPath();
      if (!Directory.Exists(pobDirPath) || !File.Exists(pobLuaBinPath))
      {
        return "";
      }
      string tmpPath = Path.GetTempPath();
      string passivesFilePath = Path.Combine(tmpPath, Guid.NewGuid().ToString() + ".json");
      File.WriteAllText(passivesFilePath, passivesJson);
      string itemsFilePath = Path.Combine(tmpPath, Guid.NewGuid().ToString() + ".json");
      File.WriteAllText(itemsFilePath, itemsJson);
      if (!File.Exists("/lib/x86_64-linux-gnu/libreadline.so.6"))
      {
        if (File.Exists("/lib/x86_64-linux-gnu/libreadline.so.7"))
        {
          runLuaLibLinkScript("/lib/x86_64-linux-gnu", 7);
        }
        else if (File.Exists("/lib/x86_64-linux-gnu/libreadline.so.8"))
        {
          runLuaLibLinkScript("/lib/x86_64-linux-gnu", 8);
        }
      }
      string headlessPob = Path.Combine(pobDirPath, "HeadlessWrapper.lua");
      ProcessStartInfo start = new ProcessStartInfo();
      start.WindowStyle = ProcessWindowStyle.Hidden;
      start.WorkingDirectory = pobDirPath;
      start.FileName = pobLuaBinPath;
      start.Arguments = string.Format("{0} {1} {2}", headlessPob, passivesFilePath, itemsFilePath);
      start.UseShellExecute = false;
      start.RedirectStandardOutput = true;
      start.RedirectStandardError = true;
      string eOut = null;
      var p = new Process();
      p.StartInfo = start;
      p.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                                 { eOut += e.Data; });
      p.Start();
      p.BeginErrorReadLine();
      string result = p.StandardOutput.ReadToEnd();
      p.WaitForExit();
      File.Delete(passivesFilePath);
      File.Delete(itemsFilePath);
      return result;
    }
  }
}