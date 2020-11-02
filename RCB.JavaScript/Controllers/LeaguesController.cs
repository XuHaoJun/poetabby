using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RCB.JavaScript.Models;
using RCB.JavaScript.Models.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using System.Data;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Dapper;
using static Dapper.SqlMapper;
using NpgsqlTypes;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

using System.Data.HashFunction;
using System.Data.HashFunction.xxHash;

using RCB.JavaScript.Services;

namespace RCB.JavaScript.Controllers
{
  public class JsonConvertHandler<T> : SqlMapper.TypeHandler<T>
  {
    public override T Parse(object value)
    {
      return JsonConvert.DeserializeObject<T>((string)value);
    }

    public override void SetValue(IDbDataParameter parameter, T value)
    {
      parameter.Value = JsonConvert.SerializeObject(value);
    }
  }

  public class CharEntry
  {
    public string CharacterId { get; set; }
    public string CharacterName { get; set; }
    public int Level { get; set; }
    public string Class { get; set; }
    public string LeagueName { get; set; }
    public string AccountName { get; set; }
    public long LifeUnreserved { get; set; }
    public long EnergyShield { get; set; }
    public PoeDepth Depth { get; set; }
    public string TreeNodes { get; set; }
    public DateTime UpdatedAt { get; set; }
  }

  public class CharacterAnalysis
  {
    public long Total { get; set; }
    public CharEntry[] Entries { get; set; }
    public long TotalUniqueCount { get; set; }
    public UniqueEntry[] UniqueCountEntries { get; set; }
    public long TotalMainSkillCount { get; set; }
    public SkillEntry[] MainSkillCountEntries { get; set; }
    public long TotalAllSkillCount { get; set; }
    public SkillEntry[] AllSkillCountEntries { get; set; }
    public long TotalAllKeystoneCount { get; set; }
    public KeystoneEntry[] AllKeystoneCountEntries { get; set; }
    public long TotalWeaponTypeCount { get; set; }
    public WeaponTypeEntry[] WeaponTypeCountEntries { get; set; }
    public long TotalClassCount { get; set; }
    public ClassEntry[] ClassCountEntries { get; set; }
    public MainSkillSupportCountEntry[] MainSkillSupportCountEntries { get; set; }
  }

  public class JsonParameter : ICustomQueryParameter
  {
    private readonly string _value;

    public JsonParameter(string value)
    {
      _value = value;
    }

    public void AddParameter(IDbCommand command, string name)
    {
      var parameter = new NpgsqlParameter(name, NpgsqlDbType.Json);
      parameter.Value = _value;

      command.Parameters.Add(parameter);
    }
  }

  public class JsonbParameter : ICustomQueryParameter
  {
    private readonly string _value;

    public JsonbParameter(string value)
    {
      _value = value;
    }

    public void AddParameter(IDbCommand command, string name)
    {
      var parameter = new NpgsqlParameter(name, NpgsqlDbType.Jsonb);
      parameter.Value = _value;

      command.Parameters.Add(parameter);
    }
  }

  // Warning
  // jsonpath datatype not implemented in npgsql, you must manual use @YourVar::jsonpath
  // or find a way implemente dapper's ICustomQueryParameter with jsonpath.
  // https://github.com/npgsql/npgsql/issues/2504

  public class MyJsonUtils
  {
    public static string Escape(string str)
    {
      // return JsonConvert.SerializeObject(str);
      return JsonSerializer.Serialize(str);
    }
  }


  [ApiController]
  [Route("api/[controller]")]
  public class LeaguesController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly PoeDbContext PoeContext;
    private readonly IDistributedCache distributedCache;
    private readonly LeagueService leagueService;

    public LeaguesController(PoeDbContext context, LeagueService _leagueService, IDistributedCache distributedCache, ILogger<LeaguesController> logger)
    {
      PoeContext = context;
      this.distributedCache = distributedCache;
      this._logger = logger;
      this.leagueService = _leagueService;

      // TODO
      // move to Startup file or dbcontext?
      SqlMapper.AddTypeHandler<PoeAccount>(new JsonConvertHandler<PoeAccount>());

      SqlMapper.AddTypeHandler<List<CharEntry>>(new JsonConvertHandler<List<CharEntry>>());
      SqlMapper.AddTypeHandler<List<UniqueEntry>>(new JsonConvertHandler<List<UniqueEntry>>());
      SqlMapper.AddTypeHandler<List<SkillEntry>>(new JsonConvertHandler<List<SkillEntry>>());
      SqlMapper.AddTypeHandler<List<KeystoneEntry>>(new JsonConvertHandler<List<KeystoneEntry>>());
      SqlMapper.AddTypeHandler<List<WeaponTypeEntry>>(new JsonConvertHandler<List<WeaponTypeEntry>>());
      SqlMapper.AddTypeHandler<List<ClassEntry>>(new JsonConvertHandler<List<ClassEntry>>());

      SqlMapper.AddTypeHandler<CharEntry[]>(new JsonConvertHandler<CharEntry[]>());
      SqlMapper.AddTypeHandler<UniqueEntry[]>(new JsonConvertHandler<UniqueEntry[]>());
      SqlMapper.AddTypeHandler<SkillEntry[]>(new JsonConvertHandler<SkillEntry[]>());
      SqlMapper.AddTypeHandler<KeystoneEntry[]>(new JsonConvertHandler<KeystoneEntry[]>());
      SqlMapper.AddTypeHandler<WeaponTypeEntry[]>(new JsonConvertHandler<WeaponTypeEntry[]>());
      SqlMapper.AddTypeHandler<ClassEntry[]>(new JsonConvertHandler<ClassEntry[]>());
      SqlMapper.AddTypeHandler<MainSkillSupportCountEntry[]>(new JsonConvertHandler<MainSkillSupportCountEntry[]>());

    }

    public class GetCharactersConfig
    {
      [FromQuery(Name = "item")]
      public List<string> items { get; set; }

      [FromQuery(Name = "class")]
      public List<string> classes { get; set; }

      [FromQuery(Name = "mainSkill")]
      public List<string> mainSkills { get; set; }

      [FromQuery(Name = "mainSkillSupport")]
      public List<string> mainSkillSupportsRaw { get; set; }
      public List<List<string>> mainSkillSupports { get; set; }

      [FromQuery(Name = "allSkill")]
      public List<string> allSkills { get; set; }

      [FromQuery(Name = "keystone")]
      public List<string> keystones { get; set; }

      [FromQuery(Name = "orderBy")]
      public List<string> sort { get; set; }

      [FromQuery(Name = "weaponType")]
      public List<string> weaponTypes { get; set; }

      [FromQuery(Name = "offhandType")]
      public List<string> offhandTypes { get; set; }
      public bool disableWeaponPairMatch { get; set; } = false;

      [FromQuery(Name = "characterNameLike")]
      [MaxLength(50)]
      public string characterNameLike { get; set; } = "";

      [FromQuery(Name = "characterId")]
      [MaxLength(64)]
      public string characterId { get; set; } = "";



      [Range(0, 50)]
      public int limit { get; set; } = 50;

      [FromQuery]
      [Range(0, Int32.MaxValue)]
      public int offset { get; set; } = 0;

      [FromQuery(Name = "levelOffset")]
      public List<int> levelOffset { get; set; }

      public void Normalized()
      {
        classes = classes != null ? classes.Where(x => !String.IsNullOrEmpty(x)).ToList() : classes;
        items = items != null ? items.Where(x => !String.IsNullOrEmpty(x)).ToList() : items;
        mainSkills = mainSkills != null ? mainSkills.Where(x => !String.IsNullOrEmpty(x)).ToList() : mainSkills;
        allSkills = allSkills != null ? allSkills.Where(x => !String.IsNullOrEmpty(x)).ToList() : allSkills;
        keystones = keystones != null ? keystones.Where(x => !String.IsNullOrEmpty(x)).ToList() : keystones;
        weaponTypes = weaponTypes != null ? weaponTypes.Where(x => !String.IsNullOrEmpty(x)).ToList() : weaponTypes;
        offhandTypes = offhandTypes != null ? offhandTypes.Where(x => !String.IsNullOrEmpty(x)).ToList() : offhandTypes;
        characterNameLike = String.IsNullOrWhiteSpace(characterNameLike) ? null : characterNameLike.Trim();
        mainSkillSupports = mainSkillSupportsRaw != null ? mainSkillSupportsRaw.Select(xsStr => xsStr.Split(',').ToList()).ToList() : null;
      }


      static public (string, bool) GetIncludeExclude(string x)
      {
        if (x.ElementAtOrDefault(0) == '!')
        {
          return (x.Substring(1), false);
        }
        else
        {
          return (x, true);
        }
      }

      static public (List<string>, List<string>) GetIncludeExcludes(List<string> xs)
      {
        List<string> includes = new List<string> { };
        List<string> excludes = new List<string> { };
        foreach (var x in xs)
        {
          var (xx, isInclude) = GetIncludeExclude(x);
          if (isInclude)
          {
            includes.Add(xx);
          }
          else
          {
            excludes.Add(xx);
          }
        }
        return (includes, excludes);
      }

      public string GetCacheKey()
      {
        List<string> all = new List<string> { };
        if (characterId != null)
        {
          all.Add(characterId);
        }
        foreach (var ls in new List<string>[] {
          items, classes, mainSkills, allSkills, keystones, sort, weaponTypes, offhandTypes
         })
        {
          if (ls != null)
          {
            all.AddRange(ls.OrderBy(x => x));
          }
        }
        if (mainSkillSupports != null)
        {
          foreach (var ls in mainSkillSupports.OrderBy(x => x))
          {
            if (ls != null)
            {
              all.AddRange(ls.OrderBy(x => x));
            }
          }
        }
        all.Add($"{limit}");
        all.Add($"{offset}");
        all.Sort();
        string keyNoHash = String.Join(",", all);
        string key = xxHashFactory.Instance.Create().ComputeHash(keyNoHash).AsHexString();
        return key;
      }
    }

    [HttpGet("")]
    public async Task<ActionResult<List<PoeLeagueModel>>> GetLeagues()
    {
      var leagues = await PoeContext.Leagues.ToListAsync();
      if (leagues == null || leagues.Count == 0)
      {
        return NotFound();
      }
      else
      {
        return leagues;
      }
    }

    private static string DEFAULT_LEAGUE_NAME = "defaultLeague";

    [HttpGet("{leagueName}")]
    public async Task<ActionResult<PoeLeagueModel>> GetDefaultLeague(string leagueName)
    {
      if (leagueName == DEFAULT_LEAGUE_NAME)
      {
        var defaultLeague = await leagueService.GetDefaultLeague();
        if (defaultLeague == null)
        {
          return NotFound();
        }
        else
        {
          return defaultLeague;
        }
      }
      else
      {
        var league = await PoeContext.Leagues.Where(x => x.LeagueId == leagueName).FirstOrDefaultAsync();
        if (league == null)
        {
          return NotFound();
        }
        else
        {
          return league;
        }
      }
    }

    private static string CHARACTERS_CACHE_PREFIX = "poetabby:Leagues:CharactersCache";

    [HttpGet("{leagueName}/characters")]
    public async Task<ActionResult<CharacterAnalysis>> GetCharacters(
          [FromRoute(Name = "leagueName")] string _leagueName,
          [FromQuery] GetCharactersConfig config
    )
    {
      config.Normalized();
      string leagueName;
      if (_leagueName == DEFAULT_LEAGUE_NAME)
      {
        PoeLeagueModel defaultLeague = await leagueService
            .GetDefaultLeagueQuery()
            .Select(x => new PoeLeagueModel { LeagueId = x.LeagueId })
            .SingleOrDefaultAsync();
        if (defaultLeague != null)
        {
          leagueName = defaultLeague.LeagueId;
        }
        else
        {
          leagueName = _leagueName;
        }
      }
      else
      {
        leagueName = _leagueName;
      }
      var cacheKey = $"{CHARACTERS_CACHE_PREFIX}:{leagueName}:{config.GetCacheKey()}";
      var encodedResult = await distributedCache.GetAsync(cacheKey);
      if (encodedResult != null)
      {
        var jsonStr = System.Text.Encoding.UTF8.GetString(encodedResult);
        var result = JsonSerializer.Deserialize<CharacterAnalysis>(jsonStr);
        Response.Headers.Add("Cache-Control", "public, max-age=60");
        return result;
      }
      else
      {
        var result = await leagueService.GetCharactersByAnalysis(leagueName, config);
        if (result.Total == 0)
        {
          return NotFound();
        }
        else
        {
          int expirMulti = (int)(result.Total / 1000);
          expirMulti = expirMulti < 1 ? 1 : expirMulti;
          expirMulti = expirMulti >= 2 ? 2 : expirMulti;
          var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(result);
          var options = new DistributedCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(3 * expirMulti))
                        .SetAbsoluteExpiration(DateTime.Now.AddHours(1 * expirMulti));
          await distributedCache.SetAsync(cacheKey, jsonBytes, options);
          Response.Headers.Add("Cache-Control", "public, max-age=180");
          return result;
        }
      }
    }
  }
}