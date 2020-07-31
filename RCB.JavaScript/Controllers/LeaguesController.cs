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

  public class UniqueEntry
  {
    public string ItemName { get; set; }
    public string ItemType { get; set; }
    public int Count { get; set; }
  }
  public class SkillEntry
  {
    public string GemId { get; set; }
    public string SkillId { get; set; }
    public string NameSpec { get; set; }
    public int Count { get; set; }
  }

  public class KeystoneEntry
  {
    public string SkillId { get; set; }
    public int Count { get; set; }
  }

  public class WeaponTypeEntry
  {
    public string WeaponType { get; set; }
    public string OffhandType { get; set; }
    public int Count { get; set; }
  }

  public class ClassEntry
  {
    public string Class { get; set; }
    public int Count { get; set; }
  }

  public class MainSkillSupportCountEntry
  {
    public string MainSkillId { get; set; }
    public List<SkillEntry> SupportCountEntries { get; set; }
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
      return JsonConvert.SerializeObject(str);
    }
  }


  [ApiController]
  [Route("api/[controller]")]
  public class LeaguesController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly PoeDbContext PoeContext;
    private readonly IDistributedCache distributedCache;

    public LeaguesController(PoeDbContext context, IDistributedCache distributedCache, ILogger<LeaguesController> logger)
    {
      PoeContext = context;
      this.distributedCache = distributedCache;
      this._logger = logger;
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

      [FromQuery(Name = "allSkill")]
      public List<string> allSkills { get; set; }

      [FromQuery(Name = "keystone")]
      public List<string> keystones { get; set; }

      [FromQuery(Name = "orderBy")]
      public List<string> sort { get; set; }

      [FromQuery(Name = "weaponType")]
      [MaxLength(20)]
      public string weaponType { get; set; }

      [FromQuery(Name = "offhandType")]
      [MaxLength(20)]
      public string offhandType { get; set; }

      [FromQuery(Name = "characterNameLike")]
      [MaxLength(50)]
      public string characterNameLike { get; set; } = "";

      [FromQuery]
      [Range(0, 50)]
      public int limit { get; set; } = 50;

      [FromQuery]
      [Range(0, Int32.MaxValue)]
      public int offset { get; set; } = 0;

      public void Normalized()
      {
        classes = classes != null ? classes.Where(x => !String.IsNullOrEmpty(x)).ToList() : classes;
        items = items != null ? items.Where(x => !String.IsNullOrEmpty(x)).ToList() : items;
        mainSkills = mainSkills != null ? mainSkills.Where(x => !String.IsNullOrEmpty(x)).ToList() : mainSkills;
        allSkills = allSkills != null ? allSkills.Where(x => !String.IsNullOrEmpty(x)).ToList() : allSkills;
        keystones = keystones != null ? keystones.Where(x => !String.IsNullOrEmpty(x)).ToList() : keystones;
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

      public string getCacheKey(string leagueName)
      {
        List<string> all = new List<string> { };
        all.Add(leagueName);
        all.AddRange(items ?? new List<string> { });
        all.AddRange(classes ?? new List<string> { });
        all.AddRange(mainSkills ?? new List<string> { });
        all.AddRange(allSkills ?? new List<string> { });
        all.AddRange(keystones ?? new List<string> { });
        all.AddRange(sort ?? new List<string> { });
        all.Add(weaponType ?? "");
        all.Add(offhandType ?? "");
        all.Add($"{limit}");
        all.Add($"{offset}");
        return String.Join("&", all);
      }
    }

    [HttpGet("{leagueName}/characters")]
    public async Task<ActionResult<CharacterAnalysis>> GetCharacters(
          string leagueName,
          [FromQuery] GetCharactersConfig config,
         [FromQuery(Name = "mainSkillSupport")] List<string> mainSkillSupports,
         [FromQuery(Name = "allSkill")] List<string> allSkills,
         [FromQuery(Name = "keystone")] List<string> keystones,
         [FromQuery(Name = "characterNameLike")][MaxLength(50)] string _characterNameLike = ""
    )
    {
      config.Normalized();
      var classes = config.classes;
      var items = config.items;
      var mainSkills = config.mainSkills;
      var weaponType = config.weaponType;
      var offhandType = config.offhandType;
      var limit = config.limit;
      var offset = config.offset;
      var sort = config.sort;
      var cacheKey = config.getCacheKey(leagueName);
      var encodedResult = await distributedCache.GetAsync(cacheKey);
      if (encodedResult != null)
      {
        var jsonStr = System.Text.Encoding.UTF8.GetString(encodedResult);
        var result = JsonSerializer.Deserialize<CharacterAnalysis>(jsonStr);
        return result;
      }
      else
      {
        string characterNameLike = String.IsNullOrWhiteSpace(_characterNameLike) ? null : _characterNameLike.Trim();
        var filtedCsBuilder = new SqlBuilder();
        var filtedCsTemplate = filtedCsBuilder.AddTemplate(@"
SELECT
	""cs"".*
FROM 
	""Characters"" AS ""cs""
 /**where**/
 /**orderby**/
			"
        );
        DynamicParameters parameter = new DynamicParameters();
        parameter.Add("@Offset", offset);
        parameter.Add("@Limit", limit);
        parameter.Add("@LeagueName", leagueName);
        filtedCsBuilder.Where(@"
""cs"".""LeagueName"" = @LeagueName
			");
        if (classes != null && classes.Count > 0)
        {
          var (includes, excludes) = GetCharactersConfig.GetIncludeExcludes(classes);
          void addFilter(List<string> xs, bool isInclude)
          {
            var op = isInclude ? "" : "NOT";
            var prefix = isInclude ? "Include" : "Exclude";
            var sqlVar = $@"@{prefix}ClassNames";
            parameter.Add(sqlVar, xs);
            filtedCsBuilder.Where($@"
{op} ""cs"".""Class"" = ANY({sqlVar})
			");
          }
          if (excludes.Count > 0)
          {
            addFilter(excludes, false);
          }
          if (includes.Count > 0)
          {
            addFilter(includes, true);
          }
        }
        if (!String.IsNullOrWhiteSpace(characterNameLike))
        {
          parameter.Add("@CharacterNameLike", characterNameLike);
          filtedCsBuilder.Where(@"
""cs"".""CharacterName"" LIKE ('%' || @CharacterNameLike || '%')
				");
        }
        if (items != null && items.Count > 0)
        {
          var (includes, excludes) = GetCharactersConfig.GetIncludeExcludes(items);
          void addFilter(List<string> xs, bool isInclude)
          {
            var op = isInclude ? "" : "NOT";
            var prefix = isInclude ? "Include" : "Exclude";
            var joined = String.Join(",", xs.Select((item) => $@"{{""name"":{MyJsonUtils.Escape(item)}}}").ToArray());
            var sqlVar = $@"@{prefix}Items";
            parameter.Add(sqlVar, new JsonbParameter($@"[{joined}]"));
            filtedCsBuilder.Where($@"
{op}(""cs"".""Items"" @> {sqlVar})
				");
          }
          if (excludes.Count > 0)
          {
            addFilter(excludes, false);
          }
          if (includes.Count > 0)
          {
            addFilter(includes, true);
          }
        }
        if (mainSkills != null && mainSkills.Count > 0)
        {
          var (includes, excludes) = GetCharactersConfig.GetIncludeExcludes(mainSkills);
          void addFilter(List<string> xs, bool isInclude)
          {
            for (int i = 0; i < xs.Count; i++)
            {
              var op = isInclude ? "" : "NOT";
              var prefix = isInclude ? "Include" : "Exclude";
              var mainSkillJsonPathVar = $"@{prefix}MainSkillJsonPath{i}";
              var mainSkillJsonPath = $@" $.Gem[*] ? (@.skillId == {MyJsonUtils.Escape(xs[i])}) ";
              parameter.Add(mainSkillJsonPathVar, mainSkillJsonPath);
              filtedCsBuilder.Where($@"
	""cs"".""CharacterId"" {op} IN (
		SELECT ""cs2"".""CharacterId""
		FROM ""Characters"" AS ""cs2"",
			 jsonb_array_elements(""cs2"".""Pob"" -> 'Skills' -> 'Skill') AS ""skills""
		WHERE
			jsonb_array_length(""skills"" -> 'Gem') >= 5
			AND
		 jsonb_path_exists(""skills"", {mainSkillJsonPathVar}::jsonpath)
	)
				");
            }
          }
          if (excludes.Count > 0)
          {
            addFilter(excludes, false);
          }
          if (includes.Count > 0)
          {
            addFilter(includes, true);
          }
        }
        if (allSkills != null && allSkills.Count > 0)
        {
          var (includes, excludes) = GetCharactersConfig.GetIncludeExcludes(allSkills);
          void addFilter(List<string> xs, bool isInclude)
          {
            var op = isInclude ? "==" : "!=";
            for (int i = 0; i < xs.Count; i++)
            {
              filtedCsBuilder.Where($@"
""cs"".""Pob"" -> 'Skills' -> 'Skill' @? '$.Gem[*] ? (@.skillId {op} {MyJsonUtils.Escape(xs[i])})'
				");
            }
          }
          if (excludes.Count > 0)
          {
            addFilter(excludes, false);
          }
          if (includes.Count > 0)
          {
            addFilter(includes, true);
          }
        }
        if (keystones != null && keystones.Count > 0)
        {
          var (includes, excludes) = GetCharactersConfig.GetIncludeExcludes(keystones);
          void addFilter(List<string> xs, bool isInclude)
          {
            var op = isInclude ? "" : "NOT";
            parameter.Add("@KeystoneIds", xs);
            filtedCsBuilder.Where($@"
{op} (regexp_split_to_array(
	btrim((""cs"".""Pob"" -> 'Tree' -> 'Spec' -> 'nodes')::TEXT, '""'),
	',+'
	) @> @KeystoneIds)
				");
          }
          if (excludes.Count > 0)
          {
            addFilter(excludes, false);
          }
          if (includes.Count > 0)
          {
            addFilter(includes, true);
          }
        }
        if (!String.IsNullOrWhiteSpace(weaponType))
        {
          void addFilter(string x, bool isInclude)
          {
            var op = isInclude ? "" : "NOT";
            if (x == "None")
            {
              var itemJsonPath = $@"'$[*] ? (@.inventoryId == ""Weapon"")'";
              filtedCsBuilder.Where($@"
{op} (NOT jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
            else
            {
              var itemJsonPath = $@"'$[*] ? (@.properties[*].name == {MyJsonUtils.Escape(x)} && @.inventoryId == ""Weapon"")'";
              filtedCsBuilder.Where($@"
{op} (jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
          }
          var (x, isInclude) = GetCharactersConfig.GetIncludeExclude(weaponType);
          addFilter(x, isInclude);
        }
        if (!String.IsNullOrWhiteSpace(offhandType))
        {
          void addFilter(string x, bool isInclude)
          {
            var op = isInclude ? "" : "NOT";
            if (x == "None")
            {
              var itemJsonPath = $@"'$[*] ? (@.inventoryId == ""Offhand"")'";
              filtedCsBuilder.Where($@"
{op} (NOT jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
            else if (x == "Shield")
            {
              var itemJsonPath = $@"'$[*] ? (@.properties[*].name == ""Armour"" && @.inventoryId == ""Offhand"")'";
              filtedCsBuilder.Where($@"
{op} (jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
            else if (x == "Quiver")
            {
              var itemJsonPath = $@"'$[*] ? (@.icon like_regex "".+/image/Art/2DItems/Quivers.+"" && @.inventoryId == ""Offhand"")'";
              filtedCsBuilder.Where($@"
{op} (jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
            else
            {
              var itemJsonPath = $@"'$[*] ? (@.properties[*].name == {MyJsonUtils.Escape(x)} && @.inventoryId == ""Offhand"")'";
              filtedCsBuilder.Where($@"
{op} (jsonb_path_exists(""cs"".""Items"", {itemJsonPath}))
				");
            }
          }
          var (x, isInclude) = GetCharactersConfig.GetIncludeExclude(offhandType);
          addFilter(x, isInclude);
        }
        if (sort != null && sort.Count > 0)
        {
          string _sortField = sort.ElementAtOrDefault(0) != null ? sort.ElementAtOrDefault(0).ToLower() : null;
          string _sortArrange = sort.ElementAtOrDefault(1) != null ? sort.ElementAtOrDefault(1).ToUpper() : null;
          string sortField;
          if (_sortField != "level" && _sortField != "es" && _sortField != "life")
          {
            sortField = "Level";
          }
          else if (_sortField == "level")
          {
            sortField = "Level";
          }
          else if (_sortField == "life")
          {
            sortField = "LifeUnreserved";
          }
          else if (_sortField == "es")
          {
            sortField = "EnergyShield";
          }
          else
          {
            sortField = "Level";
          }
          string sortArrange;
          if (_sortArrange != "DESC" && _sortArrange != "ASC")
          {
            sortArrange = "DESC";
          }
          else
          {
            sortArrange = _sortArrange;
          }
          if (sortField == "Level" && sortArrange == "DESC")
          {
            filtedCsBuilder.OrderBy($@"
""cs"".""Level"" DESC, ""cs"".""Rank"" ASC
      			");
          }
          else
          {
            filtedCsBuilder.OrderBy($@"
""cs"".""{sortField}"" {sortArrange}
      			");
          }
        }
        else
        {
          filtedCsBuilder.OrderBy($@"
""cs"".""Level"" DESC, ""cs"".""Rank"" ASC
      			");
        }
        var supportGemBuilder = new SqlBuilder();
        var supportGemTemplate = supportGemBuilder.AddTemplate(@"
SELECT
	""cs"".*
FROM 
	""FiltedCs"" AS ""cs""
 /**where**/
 /**orderby**/
			"
        );
        string mainSkillSupportSql = "";
        if (mainSkills != null && mainSkills.Count > 0)
        {
          List<string> supportSqls = mainSkills.Select((mainSkill, index) =>
          {
            var mainSkillVar = $"@MainSkillSupport{index}";
            parameter.Add(mainSkillVar, mainSkill);
            var supportSql = $@"
""_{index}_supportSource"" AS
(
SELECT
  jsonb_path_query(""skills""-> 'Gem', '$[*] ? (@.skillId like_regex ""^Support.*"")')  AS ""gemJson""
FROM
  ""FiltedCs"" AS ""cs"",
  jsonb_array_elements(""cs"".""Pob""-> 'Skills'-> 'Skill') AS ""skills""
WHERE
	""skills"" @? '$.Gem[*] ? (@.skillId == {MyJsonUtils.Escape(mainSkill)})'
),
""_{index}_supportCounts"" AS
(
SELECT
  ""_source"".""gemJson""->> 'skillId' AS ""skillId"",
  jsonb_agg(""_source"".""gemJson"")-> 0->> 'gemId' AS ""gemId"",
  jsonb_agg(""_source"".""gemJson"")-> 0->> 'nameSpec' AS ""nameSpec"",
  COUNT(*) AS ""count""
FROM
  ""_{index}_supportSource"" AS ""_source""
GROUP BY ""skillId""
ORDER BY ""count"" DESC
),
""_{index}_supportCountEntry"" AS
(
SELECT
	{mainSkillVar} AS ""mainSkillId"",
	jsonb_agg(row_to_json(""_supportCounts"")) AS ""supportCountEntries""
FROM ""_{index}_supportCounts"" AS ""_supportCounts""
)
					";
            return supportSql;
          }).ToList();
          string supportTables = String.Join(" , ", supportSqls.ToArray());
          string unionAllTable = String.Join(" UNION ALL ", mainSkills.Select((mainSkill, index) =>
          {
            return $@" SELECT * FROM ""_{index}_supportCountEntry"" ";
          }).ToArray());
          mainSkillSupportSql = $@"
""MainSkillSupportCountEntries"" AS
(
WITH
{supportTables}
SELECT jsonb_agg(row_to_json(""foo"")) AS ""mainSkillSupportCountEntries""
FROM (
{unionAllTable}
) AS ""foo""
),
        ";
        }
        var sqlb = new System.Text.StringBuilder(15000);
        sqlb.Append($@"
WITH ""FiltedCs"" AS
(
	{filtedCsTemplate.RawSql}
),");
        sqlb.Append(mainSkillSupportSql);
        sqlb.Append(@"

""MainSkillCounts"" AS
(
SELECT
	""gems"".""skillId"" AS ""skillId"",
	jsonb_agg(row_to_json(""gems"".*)) -> 0 ->> 'gemId' AS ""gemId"",
	jsonb_agg(row_to_json(""gems"".*)) -> 0 ->> 'nameSpec' AS ""nameSpec"",
	COUNT (DISTINCT ""cs"".""CharacterId"") AS ""count""
FROM 
	""FiltedCs"" AS ""cs"",
	jsonb_array_elements(""cs"".""Pob"" -> 'Skills' -> 'Skill') AS ""skills"",
	jsonb_to_recordset(
		(""skills"" -> 'Gem')::jsonb
	) AS ""gems""(""skillId"" text, ""nameSpec"" text, ""gemId"" text, ""level"" int, ""quality"" int)
WHERE
	jsonb_array_length(""skills"" -> 'Gem') >= 5
  AND
  ""skills"" ->> 'slot' NOT IN ('Weapon 1 Swap', 'Weapon 2 Swap')
GROUP BY ""gems"".""skillId""
HAVING 
	(""gems"".""skillId"" NOT LIKE 'Support%')
	AND
	(""gems"".""skillId"" NOT IN ('Spellslinger', 'Hatred', 'Wrath', 'Anger', 'Vitality'))
ORDER BY ""count"" DESC
),
""MainSkillEntries"" AS
(
SELECT jsonb_agg(row_to_json(""MainSkillCounts"")) AS ""mainSkillCountEntries""
FROM ""MainSkillCounts""
),
""TotalMainSkillCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalMainSkillCount""
FROM ""MainSkillCounts""
),

""AllSkillCounts"" AS
(
SELECT
	""gems"".""skillId"" AS ""skillId"",
	jsonb_agg(row_to_json(""gems"".*)) -> 0 ->> 'gemId' AS ""gemId"",
	jsonb_agg(row_to_json(""gems"".*)) -> 0 ->> 'nameSpec' AS ""nameSpec"",
	COUNT (DISTINCT ""cs"".""CharacterId"") AS ""count""
FROM 
	""FiltedCs"" AS ""cs"",
	jsonb_array_elements(""cs"".""Pob"" -> 'Skills' -> 'Skill') AS ""skills"",
	jsonb_to_recordset(
		(""skills"" -> 'Gem')::jsonb
	) AS ""gems""(""skillId"" text, ""nameSpec"" text, ""gemId"" text, ""level"" int, ""quality"" int)
GROUP BY ""gems"".""skillId""
ORDER BY ""count"" DESC
),
""AllSkillEntries"" AS
(
SELECT jsonb_agg(row_to_json(""AllSkillCounts"")) AS ""allSkillCountEntries""
FROM ""AllSkillCounts""
),
""TotalAllSkillCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalAllSkillCount""
FROM ""AllSkillCounts""
),

""UniqueCounts"" AS
(
SELECT 
	(""items"" ->> 'name') AS ""itemName"",
	(
		SELECT (
			CASE (""_item"" -> 0 ->> 'inventoryId')
			WHEN 'Weapon' THEN COALESCE(""_item"" -> 0 -> 'properties' -> 0 ->> 'name', 'Unknown')
			WHEN 'Offhand' THEN 
				CASE (""_item"" -> 0 -> 'properties' -> 0 ->> 'name')
				WHEN 'Quality' THEN 'Shield'
			 	WHEN 'Chance to Block' THEN 'Shield'
				ELSE
					CASE
					WHEN ""_item"" -> 0 ->> 'icon' ~ 'image/Art/2DItems/Quivers' THEN 'Quiver'
					ELSE COALESCE(""_item"" -> 0 -> 'properties' -> 0 ->> 'name', 'Unknown')
					END
				END
			WHEN 'Ring2' THEN 'Ring'
			ELSE ""_item"" -> 0 ->> 'inventoryId'
			END
			) AS ""itemType""
		FROM jsonb_agg(""items"") AS ""_item""
	),
	COUNT(DISTINCT ""cs"".""CharacterId"") AS ""count""
FROM 
	""FiltedCs"" AS ""cs"",
	jsonb_array_elements(""cs"".""Items"") AS ""items""
WHERE
	""items"" ->> 'frameType' = '3'
	AND
	""items"" ->> 'name' != ''
	AND
	""items"" ->> 'inventoryId' NOT IN('Weapon2', 'Offhand2')
GROUP BY
	""itemName""
ORDER BY ""count"" DESC
),
""UniqueCountEntries"" AS
(
SELECT jsonb_agg(row_to_json(""UniqueCounts"")) AS ""uniqueCountEntries""
FROM ""UniqueCounts""
),
""TotalUniqueCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalUniqueCount""
FROM ""UniqueCounts""
),


""AllKeystoneCounts"" AS
(
SELECT
	""nodes"" AS ""skillId"",
	COUNT(*) AS ""count""
FROM 
	""FiltedCs"" AS ""cs"",
	unnest(regexp_split_to_array(
	btrim((""cs"".""Pob"" -> 'Tree' -> 'Spec' -> 'nodes')::TEXT, '""'),
	',+'
	))
	 AS ""nodes""
WHERE
	""nodes"" IN ('258','758','772','922','1105','1697','1731','1734','1945','2598','3154','3184','3354','3554','4242','4494','4849','4917','5029','5087','5443','5819','6038','7618','8419','9271','9403','10143','10661','10808','11239','11412','11455','11490','11597','12926','12953','13374','14103','14603','14914','15435','15616','16306','16848','16940','17315','17818','18663','19083','19598','19641','19732','21264','21455','21650','22088','23090','23225','23407','23540','23572','23950','24426','24528','24720','24755','24798','24848','25309','25651','26067','27096','27604','27864','28535','28782','28884','29026','29630','29825','30847','31344','31364','31667','31700','31703','31961','32118','32249','32251','32816','32947','33645','33940','34098','34434','34484','35750','36017','37081','37127','37492','38180','38918','38999','39085','39713','39728','39790','39834','40059','40510','40561','40813','40907','41891','41970','42178','42264','42343','44297','44482','44941','45175','45313','47630','48214','48239','48480','48719','49532','49639','50692','51101','51391','51462','51492','51782','52575','53095','53816','53884','53992','54159','54307','54877','54922','55146','55509','55867','56075','56461','56722','56789','56967','57197','57257','57279','57280','57331','57560','59920','60069','60462','61259','61355','61372','61437','61627','61805','62504','62595','62817','63293','63357','63425','63490','64588','64768','65153','65296')
GROUP BY ""nodes""
ORDER BY ""count"" DESC
),
""AllKeystoneEntries"" AS
(
SELECT jsonb_agg(row_to_json(""AllKeystoneCounts"")) AS ""allKeystoneCountEntries""
FROM ""AllKeystoneCounts""
),
""TotalAllKeystoneCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalAllKeystoneCount""
FROM ""AllKeystoneCounts""
),


""WeaponTypeCounts"" AS
(
WITH ""_source"" AS
(SELECT
	jsonb_path_query(""cs"".""Items"", '$[*] ? (@.inventoryId == ""Weapon"")')
	AS ""WeaponJson"",
	jsonb_path_query(""cs"".""Items"", '$[*] ? (@.inventoryId == ""Offhand"")')
	AS ""OffhandJson""
FROM 
	""FiltedCs"" AS ""cs""
),
""_normalized"" AS
(SELECT 
	COALESCE(""_source"".""WeaponJson"" -> 'properties' -> 0 ->> 'name' , 'None') AS ""WeaponType"",
	(CASE ""_source"".""OffhandJson"" -> 'properties' -> 0 ->> 'name'
	 WHEN 'Quality' THEN 'Shield' 
	 WHEN 'Chance to Block' THEN 'Shield'
	 ELSE 
	 	CASE
		WHEN ""_source"".""OffhandJson"" ->> 'icon' ~ 'image/Art/2DItems/Quivers' THEN 'Quiver'
		ELSE COALESCE(""_source"".""OffhandJson"" -> 'properties' -> 0 ->> 'name', 'None')
		END
	 END
	) AS ""OffhandType""
FROM ""_source""
)
SELECT 
	""_normalized"".""WeaponType"",
	""_normalized"".""OffhandType"",
	COUNT(*) AS ""count""
FROM ""_normalized"" 
GROUP BY ""_normalized"".""WeaponType"", ""_normalized"".""OffhandType""
ORDER BY ""count"" DESC
),
""WeaponTypeCountEntries"" AS
(
SELECT jsonb_agg(row_to_json(""WeaponTypeCounts"")) AS ""weaponTypeCountEntries""
FROM ""WeaponTypeCounts""
),
""TotalWeaponTypeCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalWeaponTypeCount""
FROM ""WeaponTypeCounts""
),


""ClassCounts"" AS
(
SELECT
	""cs"".""Class"" AS ""class"",
	COUNT(*) AS ""count""
FROM ""FiltedCs"" AS ""cs""
GROUP BY ""cs"".""Class""
ORDER BY ""count"" DESC
),
""ClassCountEntries"" AS
(
SELECT jsonb_agg(row_to_json(""ClassCounts"")) AS ""classCountEntries""
FROM ""ClassCounts""
),
""TotalClassCount"" AS
(
SELECT 
	SUM(""count"") AS ""totalClassCount""
FROM ""ClassCounts""
),


""PagedChars"" AS
(
SELECT *
FROM ""FiltedCs""
LIMIT @Limit
OFFSET @Offset
),


""TotalCount"" AS
(
SELECT 
	COUNT(*) AS ""total""
FROM ""FiltedCs""
),


""PagedEntries"" AS
(
SELECT 
	jsonb_agg(
    json_build_object(
      'CharacterId', ""cs"".""CharacterId"",
      'CharacterName', ""cs"".""CharacterName"",
      'Level', ""cs"".""Level"",
      'Class', ""cs"".""Class"",
      'LeagueName', ""cs"".""LeagueName"",
      'AccountName', ""cs"".""AccountName"",
      'LifeUnreserved', ""cs"".""LifeUnreserved"",
      'EnergyShield', ""cs"".""EnergyShield"",
      'Depth', ""cs"".""Depth"",
      'TreeNodes', ""cs"".""Pob"" -> 'Tree' -> 'Spec' ->> 'nodes',
      'UpdatedAt', ""cs"".""UpdatedAt""
      )) AS ""entries""
FROM ""PagedChars"" AS ""cs""
)
			");
        string mainSkillSupportTableName = !String.IsNullOrWhiteSpace(mainSkillSupportSql) ?
                                          @", ""MainSkillSupportCountEntries"""
                                          : "";
        string collectAllSql = $@"
SELECT *
FROM 
""TotalCount"", ""PagedEntries"",
""TotalClassCount"", ""ClassCountEntries"",
""TotalUniqueCount"", ""UniqueCountEntries"",
""TotalMainSkillCount"", ""MainSkillEntries"",
""TotalAllSkillCount"", ""AllSkillEntries"",
""TotalAllKeystoneCount"", ""AllKeystoneEntries"",
""TotalWeaponTypeCount"", ""WeaponTypeCountEntries"" {mainSkillSupportTableName}
			";
        sqlb.Append(collectAllSql);
        using (NpgsqlConnection conn = new NpgsqlConnection(PoeContext.Database.GetDbConnection().ConnectionString))
        {
          CharacterAnalysis result = (await Dapper.SqlMapper.QueryAsync<CharacterAnalysis>(
           conn,
           sqlb.ToString(),
           parameter
           )).FirstOrDefault();
          if (result.Total == 0)
          {
            return NotFound();
          }
          else
          {
            if (result.Total >= 300)
            {
              int expirMulti = (int)(result.Total / 300);
              expirMulti = expirMulti < 1 ? 1 : expirMulti;
              expirMulti = expirMulti >= 10 ? 10 : expirMulti;
              var jsonBytes = JsonSerializer.SerializeToUtf8Bytes(result);
              var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(3 * expirMulti))
                            .SetAbsoluteExpiration(DateTime.Now.AddHours(1 * expirMulti));
              await distributedCache.SetAsync(cacheKey, jsonBytes, options);
            }
            return result;
          }
        }
      }
    }
  }
}