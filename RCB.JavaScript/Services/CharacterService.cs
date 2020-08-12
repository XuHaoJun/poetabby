using Microsoft.AspNetCore.Http;
using RCB.JavaScript.Infrastructure;
using RCB.JavaScript.Controllers;

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

using Microsoft.EntityFrameworkCore.Storage;

using System.Text.Json;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;
using static RCB.JavaScript.Controllers.LeaguesController;

namespace RCB.JavaScript.Services
{
  public class CharacterService : ServiceBase
  {


    public CharacterService()
    {
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

    public async Task DefaultUpsert(PoeCharacterModel poeChar)
    {
      async Task upsert(PoeDbContext PoeContext)
      {
        if (!String.IsNullOrWhiteSpace(poeChar.PobCode))
        {
          await PoeContext.Upsert(poeChar).
          On(c => c.CharacterId).
          WhenMatched((oldChar, newChar) => new PoeCharacterModel()
          {
            Level = newChar.Level,
            Class = newChar.Class,
            LifeUnreserved = newChar.LifeUnreserved,
            EnergyShield = newChar.EnergyShield,
            Dead = newChar.Dead,
            Depth = newChar.Depth,
            Account = newChar.Account,
            Online = newChar.Online,
            Rank = newChar.Rank,
            Experience = newChar.Experience,
            PobCode = newChar.PobCode,
            Pob = newChar.Pob,
            Items = newChar.Items,
            UpdatedAt = newChar.PobCode != oldChar.PobCode ? System.DateTime.UtcNow : oldChar.UpdatedAt
          }).RunAsync();
        }
      }
      var PoeContext = new PoeDbContext();
      using (var tran = PoeContext.Database.BeginTransaction())
      {
        await upsert(PoeContext);
        var config = new GetCharactersConfig { characterId = poeChar.CharacterId };
        var (sqlb, parameter) = LeagueService.GetCharactersSql(poeChar.LeagueName, config);
        var sql = sqlb.ToString();
        CharacterAnalysis result = (await PoeContext.Database.GetDbConnection().QueryAsync<CharacterAnalysis>(
          sqlb.ToString(),
          parameter,
          tran.GetDbTransaction()
         )).FirstOrDefault();
        if (result == null || result.Total != 1)
        {
          tran.Rollback();
        }
        else
        {
          var config2 = new GetCharactersConfig
          {
            characterId = poeChar.CharacterId,
            mainSkills = result.MainSkillCountEntries != null ? result.MainSkillCountEntries.Select(x => x.SkillId).ToList() : new List<string> { }
          };
          var (sqlb2, parameter2) = LeagueService.GetCharactersSql(poeChar.LeagueName, config2);
          var sql2 = sqlb2.ToString();
          CharacterAnalysis result2 = (await PoeContext.Database.GetDbConnection().QueryAsync<CharacterAnalysis>(
            sqlb2.ToString(),
            parameter2,
            tran.GetDbTransaction()
           )).FirstOrDefault();
          if (result2 == null || result2.Total != 1)
          {
            tran.Rollback();
          }
          else
          {
            CountAnalysis countAnalysis = new CountAnalysis()
            {
              ClassCountEntries = result2.ClassCountEntries ?? new ClassEntry[] { },
              UniqueCountEntries = result2.UniqueCountEntries ?? new UniqueEntry[] { },
              AllKeystoneCountEntries = result2.AllKeystoneCountEntries ?? new KeystoneEntry[] { },
              MainSkillCountEntries = result2.MainSkillCountEntries ?? new SkillEntry[] { },
              MainSkillSupportCountEntries = result2.MainSkillSupportCountEntries ?? new MainSkillSupportCountEntry[] { },
              AllSkillCountEntries = result2.AllSkillCountEntries ?? new SkillEntry[] {},
              WeaponTypeCountEntries = result2.WeaponTypeCountEntries ?? new WeaponTypeEntry[] {}
            };
            var updateCountSql = $@"UPDATE ""Characters"" SET ""CountAnalysis"" = @CountAnalysis WHERE ""CharacterId"" = @CharacterId";
            await PoeContext.Database.GetDbConnection().ExecuteAsync(updateCountSql, new
            {
              CharacterId = poeChar.CharacterId,
              CountAnalysis = new JsonbParameter(JsonSerializer.Serialize(countAnalysis))
            }, tran.GetDbTransaction());
            tran.Commit();
          }
        }
      }
    }
  }
}