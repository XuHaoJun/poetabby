using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using RCB.JavaScript.Services;
using RCB.JavaScript.Models;
using System.Linq;

namespace RCB.JavaScript.Models.Utils
{
  public class PoeFetcherManager : BackgroundService
  {
    private readonly ILogger<PoeFetcherManager> _logger;
    private readonly CharacterService characterService;
    private readonly PoeDbContext PoeContext;


    public PoeFetcherManager(ILogger<PoeFetcherManager> logger)
    {
      _logger = logger;
      this.PoeContext = new PoeDbContext();
      this.characterService = new CharacterService();
    }

    private async Task ForceUpdateCharacters(CancellationToken stoppingToken)
    {
      List<PoeCharacterModel> _chars =
        await PoeContext.Characters
        .Select(c => new PoeCharacterModel { CharacterId = c.CharacterId, LeagueName = c.LeagueName })
        .ToListAsync();
      _logger.LogInformation($"Chars Upsert Count: {_chars.Count}");
      foreach (var _c in _chars)
      {
        if (stoppingToken.IsCancellationRequested)
        {
          return;
        }
        await characterService.DefaultUpsert(_c);
      }
      // List<Task> tasks = new List<Task>();
      // foreach (var _c in _chars)
      // {
      //   tasks.Add(characterService.DefaultUpsert(_c));
      // }
      // Task.WaitAll(tasks.ToArray());
    }

    private void ForceUpdateCharactersPob(CancellationToken stoppingToken)
    {
      List<PoeCharacterModel> _chars =
         PoeContext.Characters
        .Select(c => new PoeCharacterModel { CharacterId = c.CharacterId, PobCode = c.PobCode })
        .ToList();
      foreach (var _c in _chars)
      {
        if (stoppingToken.IsCancellationRequested)
        {
          return;
        }
        try
        {
          string newPobXml = PobUtils.GetBuildXmlByXml(PobUtils.CodeToXml(_c.PobCode));
          string nextPobCode = PobUtils.XmlToCode(newPobXml);
          PathOfBuilding nextPob = PobUtils.XmlToPob(newPobXml);
          _c.PobCode = nextPobCode;
          _c.Pob = nextPob;
          _c.Pob = PobUtils.XmlToPob(PobUtils.CodeToXml(_c.PobCode));
          using (var db = new PoeDbContext())
          {
            db.Characters.Attach(_c);
            db.Entry(_c).Property(x => x.PobCode).IsModified = true;
            db.Entry(_c).Property(x => x.Pob).IsModified = true;
            db.SaveChanges();
          }
        }
        catch (System.Exception e)
        {
          _logger.LogError(e.ToString());
        }
        System.GC.Collect();
      }
    }

    private async Task fetchCharsFromLadder(string leagueName, int limit, int offset)
    {
      _logger.LogInformation($"GetCharactersFromLadder Harvest Limit: {limit}, Offset: {offset}");
      try
      {
        await foreach (PoeCharacterModel poeChar in PoeFetcher.GetCharactersFromLadderIterator(leagueName, limit, offset))
        {
          _logger.LogInformation($"Upserting {poeChar.AccountName} / {poeChar.CharacterName}");
          await characterService.DefaultUpsert(poeChar);
          System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
          System.GC.Collect();
          System.GC.WaitForPendingFinalizers();
          System.GC.Collect();
          _logger.LogInformation($"isServerGC: {System.Runtime.GCSettings.IsServerGC}");
          _logger.LogInformation($"Total memory: {System.GC.GetTotalMemory(false)}");
        }
      }
      catch (System.Net.Sockets.SocketException e)
      {
        _logger.LogDebug(e.ToString());
        _logger.LogInformation($"Failed fetch GetCharactersFromLadder {leagueName} Limit: {limit}, Offset: {offset}");
      }
    }


    private async Task infiniteRandom(string leagueName, int targetMax, int limit)
    {
      _logger.LogDebug($"Infinte Random get characters from ladder.");
      while (true)
      {
        System.Random rnd = new System.Random();
        int offset = rnd.Next(0, targetMax - limit);
        await fetchCharsFromLadder(leagueName, limit, offset);
        PobUtils.DisposeLuaState();
        System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        await Task.Delay(60000 * 5);
      }
    }


    private async Task ladderRankAscGet(string leagueName, int maxRank, int limit, int maxDelta)
    {
      for (int i = 0; i < (maxDelta / limit); i++)
      {
        int offset = (limit * i) + maxRank;
        await fetchCharsFromLadder(leagueName, limit, offset);
        // PobUtils.DisposeLuaState();
        await Task.Delay(60000 * 5);
      }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation($"PoeFetcherManager is starting.");

      stoppingToken.Register(() =>
          _logger.LogInformation($"PoeFetcherManager background task is stopping."));
      try
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

        // await ForceUpdateCharacters(stoppingToken);
        // ForceUpdateCharactersPob(stoppingToken);

        // PobUtils.Foo();
        // return;

        // await Task.Delay(1000);
        int maxRank = await PoeContext.Characters.MaxAsync(c => (int?)c.Rank) ?? 0;
        int targetMax = 15000;
        int maxDelta = targetMax - maxRank;
        int limit = 50;
        string currentLeagueName = "Harvest";


        if (maxDelta <= limit)
        {
          await infiniteRandom(currentLeagueName, targetMax, limit);
        }
        else
        {
          // await ladderRankAscGet(currentLeagueName);
          // await infiniteRandom(currentLeagueName, targetMax, limit);
        }
      }
      catch (System.Exception e)
      {
        _logger.LogError(e.ToString());
      }
    }
  }
}