using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace RCB.JavaScript.Models.Utils
{
  public class PoeFetcherManager : BackgroundService
  {
    private readonly ILogger<PoeFetcherManager> _logger;


    public PoeFetcherManager(ILogger<PoeFetcherManager> logger)
    {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation($"PoeFetcherManager is starting.");

      stoppingToken.Register(() =>
          _logger.LogInformation($"PoeFetcherManager background task is stopping."));
      try
      {
        using (var PoeContext = new PoeDbContext())
        {
          // var _chars = await PoeContext.Characters.ToListAsync();
          // foreach (var _c in _chars)
          // {
          //   foreach (var stat in _c.Pob.Build.PlayerStat)
          //   {
          //     if (stat.stat == "LifeUnreserved")
          //     {
          //       _c.LifeUnreserved = (long)stat.value;
          //     }
          //     else if (stat.stat == "EnergyShield")
          //     {
          //       _c.EnergyShield = (long)stat.value;
          //     }
          //   }
          //   if (_c.Pob.Skills == null)
          //   {
          //     _c.Pob.Skills = new PathOfBuildingSkills() { };
          //   }
          //   if (_c.Pob.Skills.Skill == null)
          //   {
          //     _c.Pob.Skills.Skill = new PathOfBuildingSkillsSkill[] { };
          //   }
          // }
          // PoeContext.UpdateRange(_chars);
          // await PoeContext.SaveChangesAsync();
          int maxRank = await PoeContext.Characters.MaxAsync(c => (int?)c.Rank) ?? 0;
          // int maxRank = 0;
          int targetMax = 15000;
          int max = targetMax - maxRank;
          int limit = 10;
          if (max <= limit)
          {
            _logger.LogDebug($"Rank reach targetMax: {targetMax}");
            while (true)
            {
              if (stoppingToken.IsCancellationRequested)
              {
                return;
              }
              System.Random rnd = new System.Random();
              int offset = rnd.Next(0, targetMax - limit);
              _logger.LogInformation($"GetCharactersFromLadder Harvest Limit: {limit}, Offset: {offset}");
              List<PoeCharacterModel> poeChars;
              try
              {
                poeChars = await PoeFetcher.GetCharactersFromLadder("Harvest", limit, offset);
              }
              catch (System.Net.Sockets.SocketException e)
              {
                _logger.LogDebug(e.ToString());
                _logger.LogInformation($"Retry(after 5min) GetCharactersFromLadder Harvest Limit: {limit}, Offset: {offset}");
                await Task.Delay(60000 * 5);
                continue;
              }
              foreach (PoeCharacterModel poeChar in poeChars)
              {
                if (stoppingToken.IsCancellationRequested)
                {
                  return;
                }
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
                  UpdatedAt = System.DateTime.UtcNow
                }).RunAsync();
              }
              await Task.Delay(60000 * 10);
            }
          }
          else
          {
            for (int i = 0; i < (max / limit); i++)
            {
              if (stoppingToken.IsCancellationRequested)
              {
                return;
              }
              var offset = (limit * i) + maxRank;
              _logger.LogInformation($"GetCharactersFromLadder Harvest Limit: {limit}, Offset: {offset}");
              List<PoeCharacterModel> poeChars;
              try
              {
                poeChars = await PoeFetcher.GetCharactersFromLadder("Harvest", limit, offset);
              }
              catch (System.Net.Sockets.SocketException e)
              {
                _logger.LogDebug(e.ToString());
                _logger.LogInformation($"Retry(after 5min) GetCharactersFromLadder Harvest Limit: {limit}, Offset: {offset}");
                // retry after 5min
                i--;
                await Task.Delay(60000 * 5);
                continue;
              }
              foreach (PoeCharacterModel poeChar in poeChars)
              {
                if (stoppingToken.IsCancellationRequested)
                {
                  return;
                }
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
                  UpdatedAt = System.DateTime.UtcNow
                }).RunAsync();
              }
              await Task.Delay(60000 * 10);
            }
          }
        }
      }
      catch (System.Exception e)
      {
        _logger.LogError(e.ToString());
      }
    }
  }
}