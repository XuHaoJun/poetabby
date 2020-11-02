using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RCB.JavaScript.Models.Utils
{
  public class LadderCharacterResponse
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public int Level { get; set; }
    public string Class { get; set; }
    public int Score { get; set; }
    public long Experience { get; set; }
    public PoeDepth Depth { get; set; }
  }

  public class LadderEntry
  {
    public int Rank { get; set; }
    public bool Dead { get; set; }
    public bool Online { get; set; }
    public LadderCharacterResponse Character { get; set; }
    public PoeAccount Account { get; set; }
  }

  public class LadderResponse
  {
    public int Total { get; set; }
    public System.DateTime Cached_since { get; set; }
    public List<LadderEntry> Entries { get; set; }
  }

  public class PoeFetcher
  {
    private static readonly HttpClient HttpClient = new HttpClient();

    static public string GetCharacterItemsUrl(string accountName, string characterName)
    {
      string urlParameters = $"accountName={accountName}&character={characterName}";
      return $"http://www.pathofexile.com/character-window/get-items?{urlParameters}";
    }

    static public string GetCharacterPassivesUrl(string accountName, string characterName)
    {
      string urlParameters = $"accountName={accountName}&character={characterName}&reqData=0";
      return $"http://www.pathofexile.com/character-window/get-passive-skills?{urlParameters}";
    }

    static public string GetLadderUrl(string leagueName, int limit, int offset, string _accountName)
    {
      string urlParameters = (offset > 0 ? $"offset={offset}" : "") + (offset > 0 ? "&" : "") + (limit > 0 ? $"limit={limit}" : "");
      urlParameters += _accountName != null ? $"&accountName={_accountName}" : "";
      return $"http://www.pathofexile.com/api/ladders/{leagueName}?{urlParameters}";
    }

    static public string GetLeaguesUrl()
    {
      return $"https://www.pathofexile.com/api/leagues?realm=pc&compat=0";
    }

    public class LeagueResponse : PoeLeagueModel
    {
      public string Id { get; set; }

      public PoeLeagueModel ToPoeLeagueModel()
      {
        var model = (PoeLeagueModel)this;
        model.LeagueId = this.Id;
        return model;
      }
    }

    static public async Task<List<PoeLeagueModel>> GetLeagues()
    {
      List<LeagueResponse> leagues;
      using (var httpRes = await HttpClient.GetAsync(GetLeaguesUrl()))
      {
        if (httpRes.IsSuccessStatusCode)
        {
          leagues = JsonConvert.DeserializeObject<List<LeagueResponse>>(await httpRes.Content.ReadAsStringAsync());
        }
        else
        {
          leagues = new List<LeagueResponse> { };
        }
      }
      return leagues.Select(x => x.ToPoeLeagueModel()).ToList();
    }

    static public async IAsyncEnumerable<CharFetchResultPayload> GetCharFetchResultPayloadFromLadderIterator(
      string leagueName, int limit = 10, int offset = 0, string _accountName = null)
    {
      string urlParams = (offset > 0 ? $"offset={offset}" : "") + (offset > 0 ? "&" : "") + (limit > 0 ? $"limit={limit}" : "");
      urlParams += _accountName != null ? $"&accountName={_accountName}" : "";
      LadderResponse ladderResponse;
      using (var httpLadderRes = await HttpClient.GetAsync(GetLadderUrl(leagueName, limit, offset, _accountName)))
      {
        if (httpLadderRes.IsSuccessStatusCode)
        {
          string ladderBody = await httpLadderRes.Content.ReadAsStringAsync();
          ladderResponse = JsonConvert.DeserializeObject<LadderResponse>(ladderBody);
        }
        else
        {
          ladderResponse = null;
        }
      }
      if (ladderResponse != null)
      {
        for (int i = 0; i < ladderResponse.Entries.Count; i++)
        {
          LadderEntry entry = ladderResponse.Entries[i];
          string accountName = entry.Account != null ? entry.Account.Name : _accountName;

          string passivesJson;
          using (var passivesRes = await HttpClient.GetAsync(GetCharacterPassivesUrl(accountName, entry.Character.Name)))
          {
            passivesJson = passivesRes.IsSuccessStatusCode ? await passivesRes.Content.ReadAsStringAsync() : "";
          }

          if (passivesJson.Length > 0)
          {
            await Task.Delay(500);
            string itemsJson;
            using (var itemsRes = await HttpClient.GetAsync(GetCharacterItemsUrl(accountName, entry.Character.Name)))
            {
              itemsJson = itemsRes.IsSuccessStatusCode ? await itemsRes.Content.ReadAsStringAsync() : "";
            }
            if (passivesJson.Length > 0 && itemsJson.Length > 0)
            {
              yield return new CharFetchResultPayload()
              {
                leagueName = leagueName,
                ladderEntry = entry,
                passivesJson = passivesJson,
                itemsJson = itemsJson
              };
            }
          }
          await Task.Delay(500);
        }
      }
    }

    public class CharFetchResultPayload
    {
      public string leagueName { get; set; }
      public LadderEntry ladderEntry { get; set; }
      public string passivesJson { get; set; }
      public string itemsJson { get; set; }

      public PoeCharacterModel ToCharacterModel(NLua.Lua luaState)
      {
        var entry = ladderEntry;
        string buildXml = PobUtils.GetBuildXmlByJsons(passivesJson, itemsJson, luaState);
        PathOfBuilding tryGetPob()
        {
          PathOfBuilding pob;
          try
          {
            pob = PobUtils.XmlToPob(buildXml);
          }
          catch (System.Exception e)
          {
            // TODO
            // Log entry and e
            if (e is System.InvalidOperationException || e is System.Xml.XmlException)
            {
              pob = null;
              System.Console.WriteLine(e.ToString());
            }
            else
            {
              throw;
            }
          }
          return pob;
        }
        PathOfBuilding pob = tryGetPob();
        if (pob != null)
        {
          PoeItems items = JsonConvert.DeserializeObject<PoeItems>(itemsJson);
          PoePassives passives = JsonConvert.DeserializeObject<PoePassives>(passivesJson);
          string code = PobUtils.XmlToCode(buildXml);
          var poeChar = new PoeCharacterModel()
          {
            CharacterId = entry.Character.Id,
            CharacterName = entry.Character.Name,
            LeagueName = leagueName,
            Account = entry.Account,
            AccountName = entry.Account != null ? entry.Account.Name : null,
            Level = entry.Character.Level,
            Class = entry.Character.Class,
            Depth = entry.Character.Depth ?? new PoeDepth { Solo = 0, Default = 0 },
            Dead = entry.Dead,
            Online = entry.Online,
            Rank = entry.Rank,
            Experience = entry.Character.Experience,
            LifeUnreserved = pob.GetStat("LifeUnreserved"),
            EnergyShield = pob.GetStat("EnergyShield"),
            Pob = pob,
            PobCode = code,
            Items = new List<PoeItem>() { }.Concat(items.items.Concat(passives.items.Select(i => i.ToPoeItem()))).ToList(),
            UpdatedAt = System.DateTime.UtcNow,
          };
          return poeChar;
        }
        else
        {
          return null;
        }
      }
    }
  }
}