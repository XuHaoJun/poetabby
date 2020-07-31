using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RCB.JavaScript.Models.Utils
{
  public class PobUtils
  {
    private static System.IO.MemoryStream GenerateStreamFromString(string value)
    {
      return new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(value ?? ""));
    }

    static public string GetCode(string xmlStr)
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

    static private void runLuaLibLinkScript(string workingDirectory)
    {
      Process process = new Process();
      ProcessStartInfo start = new ProcessStartInfo();

      var cmd = "ln -s libreadline.so.7 libreadline.so.6";
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

    static public string GetXml(string passivesJson, string itemsJson)
    {
      string tmpPath = Path.GetTempPath();
      string passivesFilePath = Path.Combine(tmpPath, Guid.NewGuid().ToString() + ".json");
      File.WriteAllText(passivesFilePath, passivesJson);
      string itemsFilePath = Path.Combine(tmpPath, Guid.NewGuid().ToString() + ".json");
      File.WriteAllText(itemsFilePath, itemsJson);
      bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                                                     .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
      string luabinName = isWindows ? "lua52.exe" : "lua52";
      string pobDirPath = Path.Combine(Directory.GetCurrentDirectory(), "ExtraFiles", "pob");
      string pobLuaBinPath = Path.Combine(pobDirPath, luabinName);
      if (!Directory.Exists(pobDirPath) || !File.Exists(pobLuaBinPath))
      {
        return "";
      }
      if (!File.Exists("/lib/x86_64-linux-gnu/libreadline.so.6") && File.Exists("/lib/x86_64-linux-gnu/libreadline.so.7"))
      {
        runLuaLibLinkScript("/lib/x86_64-linux-gnu");
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