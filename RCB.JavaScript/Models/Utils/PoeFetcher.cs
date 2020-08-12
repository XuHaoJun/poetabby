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
    private static readonly HttpClient HttpClient;

    static public async IAsyncEnumerable<PoeCharacterModel> GetCharactersFromLadderIterator(string leagueName, int limit = 10, int offset = 0)
    {
      var HttpClient = new HttpClient();
      string urlParams = (offset > 0 ? $"offset={offset}" : "") + (offset > 0 ? "&" : "") + (limit > 0 ? $"limit={limit}" : "");
      var httpLadderRes = await HttpClient.GetAsync($"http://www.pathofexile.com/api/ladders/{leagueName}?{urlParams}");
      if (httpLadderRes.IsSuccessStatusCode)
      {
        string ladderBody = await httpLadderRes.Content.ReadAsStringAsync();
        LadderResponse ladderResponse = JsonConvert.DeserializeObject<LadderResponse>(ladderBody);
        foreach (LadderEntry entry in ladderResponse.Entries)
        {
          string urlParameters;
          urlParameters = $"accountName={entry.Account.Name}&character={entry.Character.Name}&reqData=0";
          var passivesRes = await HttpClient.GetAsync($"http://www.pathofexile.com/character-window/get-passive-skills?{urlParameters}");
          string passivesJson = passivesRes.IsSuccessStatusCode ? await passivesRes.Content.ReadAsStringAsync() : "";
          urlParameters = $"accountName={entry.Account.Name}&character={entry.Character.Name}";
          var itemsRes = await HttpClient.GetAsync($"http://www.pathofexile.com/character-window/get-items?{urlParameters}");
          string itemsJson = itemsRes.IsSuccessStatusCode ? await itemsRes.Content.ReadAsStringAsync() : "";
          if (passivesJson.Length > 0 && itemsJson.Length > 0)
          {
            string xml = PobUtils.GetBuildXmlByJsons(passivesJson, itemsJson);
            PathOfBuilding pob;
            try
            {
              pob = PobUtils.XmlToPob(xml);
              // Use empty skills instead of null that better for sql query.
              if (pob.Skills == null)
              {
                pob.Skills = new PathOfBuildingSkills() { };
              }
              if (pob.Skills.Skill == null)
              {
                pob.Skills.Skill = new PathOfBuildingSkillsSkill[] { };
              }
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
            if (pob != null)
            {
              PoeItems items = JsonConvert.DeserializeObject<PoeItems>(itemsJson);
              PoePassives passives = JsonConvert.DeserializeObject<PoePassives>(passivesJson);
              string code = PobUtils.XmlToCode(xml);
              var poeChar = new PoeCharacterModel()
              {
                CharacterId = entry.Character.Id,
                CharacterName = entry.Character.Name,
                LeagueName = leagueName,
                Account = entry.Account,
                AccountName = entry.Account.Name,
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
              yield return poeChar;
            }
          }
          await Task.Delay(500);
        }
      }
    }

    static public async Task<List<PoeCharacterModel>> GetCharactersFromLadder(string leagueName, int limit = 10, int offset = 0)
    {
      List<PoeCharacterModel> poeChars = new List<PoeCharacterModel>();
      await foreach (PoeCharacterModel pchar in GetCharactersFromLadderIterator(leagueName, limit, offset))
      {
        poeChars.Add(pchar);
      }
      return poeChars;
    }
  }
}