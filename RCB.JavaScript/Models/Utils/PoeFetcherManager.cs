using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using RCB.JavaScript.Services;
using RCB.JavaScript.Models;
using System.Linq;
using NLua;
using Dapper;

namespace RCB.JavaScript.Models.Utils
{
  public class PoeFetcherManager : BackgroundService
  {
    private readonly ILogger<PoeFetcherManager> _logger;
    private readonly CharacterService characterService;
    private readonly LeagueService leagueService;
    private readonly PoeDbContext PoeContext;
    // private Lua luaState;


    // TODO
    // add char and league service to singleton
    public PoeFetcherManager(ILogger<PoeFetcherManager> logger)
    {
      _logger = logger;
      this.PoeContext = new PoeDbContext();
      this.characterService = new CharacterService(this.PoeContext);
      this.leagueService = new LeagueService(this.PoeContext);
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
      }
    }

    private async Task ladderTask(
      CancellationToken stoppingToken,
      string currentLeagueName, int limit = 50, int offset = 0, string accountName = null)
    {
      var payloads = new List<PoeFetcher.CharFetchResultPayload> { };
      await foreach (var payload in PoeFetcher.GetCharFetchResultPayloadFromLadderIterator(currentLeagueName, limit, offset, accountName))
      {
        if (stoppingToken.IsCancellationRequested)
        {
          return;
        }
        payloads.Add(payload);
      }
      System.GC.Collect();
      PobUtils.try_malloc_trim(0);
      // gc for run pob, need 500M+ memory.
      var pchars = new List<PoeCharacterModel> { };
      using (var luaState = PobUtils.CreateLuaState())
      {
        foreach (var payload in payloads)
        {
          if (stoppingToken.IsCancellationRequested)
          {
            break;
          }
          PoeCharacterModel pchar = payload.ToCharacterModel(luaState);
          if (pchar != null)
          {
            pchars.Add(pchar);
          }
        }
      }
      // force clear 500M+ memory
      System.GC.Collect();
      PobUtils.try_malloc_trim(0);
      foreach (var pchar in pchars)
      {
        _logger.LogInformation($"Upserting {pchar.AccountName} / {pchar.CharacterName}");
        await characterService.DefaultUpsert(pchar);
      }
    }

    private async Task FetchAndUpsertCharacters(CancellationToken stoppingToken, string targetLeagueName)
    {
      int limit = 50;
      long limitCharRows = 9000;
      long numCharRows = await characterService.GetNumCharacters();
      if (numCharRows >= limitCharRows)
      {
        _logger.LogInformation($"Full characters reach limit rows {limitCharRows}, current: {numCharRows} only update.");
        var entityType = PoeContext.Model.FindEntityType(typeof(PoeCharacterModel));
        var schema = entityType.GetSchema();
        var tableName = entityType.GetTableName();
        var sql = $@"
        SELECT ""AccountName"", ""LeagueName"", ""CharacterId"", ""Account""
        FROM ""{tableName}""
        ORDER BY RANDOM()
        LIMIT {limit}
        ";
        var pchars = PoeContext.Database.GetDbConnection().Query<PoeCharacterModel>(sql).ToList();
        var fetchResults = new List<PoeFetcher.CharFetchResultPayload> { };
        foreach (PoeCharacterModel pchar in pchars)
        {
          // Not support specific character from ladder only by accountName.
          await foreach (var res in PoeFetcher.GetCharFetchResultPayloadFromLadderIterator(pchar.LeagueName, 5, 0, pchar.AccountName))
          {
            if (res != null && res.ladderEntry != null && res.ladderEntry.Account == null)
            {
              res.ladderEntry.Account = pchar.Account;
            }
            fetchResults.Add(res);
          }
          await Task.Delay(500, stoppingToken);
        }
        fetchResults = fetchResults.Where(res =>
        {
          return pchars.Any(p =>
          {
            return p.CharacterId == res.ladderEntry.Character.Id;
          });
        }
        ).ToList();
        System.GC.Collect();
        PobUtils.try_malloc_trim(0);
        List<PoeCharacterModel> newPchars;
        using (var luaState = PobUtils.CreateLuaState())
        {
          newPchars = fetchResults
                  .Select(res => res.ToCharacterModel(luaState))
                  .Where(pchar => pchar != null)
                  .ToList();
        }
        System.GC.Collect();
        PobUtils.try_malloc_trim(0);
        foreach (var pchar in newPchars)
        {
          _logger.LogInformation($"Upserting {pchar.AccountName} / {pchar.CharacterName}");
          await characterService.DefaultUpsert(pchar);
        }
      }
      else
      {
        int currentMaxRank = await PoeContext.Characters.MaxAsync(c => (int?)c.Rank) ?? 0;
        int targetMax = 15000;
        int maxDelta = targetMax - currentMaxRank;
        string currentLeagueName = targetLeagueName;
        if (maxDelta <= limit)
        {
          System.Random rnd = new System.Random();
          int offset = rnd.Next(0, targetMax - limit);
          await ladderTask(stoppingToken, currentLeagueName, limit, offset);
        }
        else
        {
          int offset = currentMaxRank;
          await ladderTask(stoppingToken, currentLeagueName, limit, offset);
        }
      }
    }


    private async Task FetchAndUpsertLeagues()
    {
      List<PoeLeagueModel> leagues = await PoeFetcher.GetLeagues();
      using (var PoeContext = new PoeDbContext())
      {
        await PoeContext.Leagues.UpsertRange(leagues.ToArray()).On(x => new { LeagueId = x.LeagueId }).RunAsync();
      }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation($"Is starting.");

      // web server start first 
      Thread.Yield();

      var taskCancelSources = new Dictionary<Task, CancellationTokenSource> { };
      stoppingToken.Register(() =>
      {
        _logger.LogInformation($"Background task is stopping.");
        _logger.LogInformation("Stopping children background tasks.");
        foreach (var canlSource in taskCancelSources.Values)
        {
          canlSource.Cancel();
        }
      });
      try
      {
        async Task fetchAndUpsertLeaguesLoop(CancellationToken stoppingToken, PoeLeagueModel waitLeague = null)
        {
          stoppingToken.Register(() =>
          {
            _logger.LogInformation("Stopping fetchAndUpsertLeaguesLoop task");
          });
          while (!stoppingToken.IsCancellationRequested)
          {
            int defaultDelayTime = 60000 * 60 * 2; // 2hr
            if (waitLeague == null)
            {
              await Task.Delay(defaultDelayTime, stoppingToken);
            }
            else
            {
              if (waitLeague.EndAt == null)
              {
                await Task.Delay(defaultDelayTime, stoppingToken);
              }
              else
              {
                var now = System.DateTime.UtcNow;
                if (waitLeague.EndAt.Value > now)
                {
                  System.TimeSpan delayTimeSpan = waitLeague.EndAt.Value.Subtract(System.DateTime.UtcNow);
                  await Task.Delay(delayTimeSpan, stoppingToken);
                }
                else
                {
                  await Task.Delay(defaultDelayTime, stoppingToken);
                }
              }
            }
            await FetchAndUpsertLeagues();
          }
        }
        await FetchAndUpsertLeagues();

        async Task fetchAndUpsertCharactersLoop(CancellationToken stoppingToken, string targetLeagueName)
        {
          int defaultDelayTime = 60000 * 10; // 10 min
          stoppingToken.Register(() =>
          {
            _logger.LogInformation($"Stopping {targetLeagueName} fetchAndUpsertCharactersLoop task");
          });
          while (!stoppingToken.IsCancellationRequested)
          {
            await FetchAndUpsertCharacters(stoppingToken, targetLeagueName);
            await Task.Delay(defaultDelayTime, stoppingToken);
          }
        }

        var tasks = new List<Task> { };

        PoeLeagueModel defaultLeague = await leagueService.GetDefaultLeague();
        if (defaultLeague != null)
        {
          var leagueCancelSource = new CancellationTokenSource();
          Task leaguesLoop = fetchAndUpsertLeaguesLoop(leagueCancelSource.Token, defaultLeague);
          tasks.Add(leaguesLoop);
          taskCancelSources.Add(leaguesLoop, leagueCancelSource);

          var characterCancelSource = new CancellationTokenSource();
          Task charactersLoop = fetchAndUpsertCharactersLoop(characterCancelSource.Token, defaultLeague.LeagueId);
          tasks.Add(charactersLoop);
          taskCancelSources.Add(charactersLoop, characterCancelSource);
        }
        else
        {
          _logger.LogError("Not found default league!!!");
        }

        // you can add other leagues
        await Task.WhenAll(tasks.ToArray());
      }
      catch (System.Exception e)
      {
        _logger.LogError(e.ToString());
      }
      finally
      {
        foreach (var canlSource in taskCancelSources.Values)
        {
          canlSource.Dispose();
        }
      }
    }
  }
}