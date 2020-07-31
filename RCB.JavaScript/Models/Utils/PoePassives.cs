using System.Linq;

namespace RCB.JavaScript.Models.Utils
{
  public class PoePassives
  {
    public object[] hashes { get; set; }
    public System.Collections.Generic.List<PoePassivesItem> items { get; set; }
    public object[] visual_overrides { get; set; }
  }

  public class PoePassivesItem
  {
    public bool verified { get; set; }
    public int w { get; set; }
    public int h { get; set; }
    public string icon { get; set; }
    public string league { get; set; }
    public string id { get; set; }
    public string name { get; set; }
    public string typeLine { get; set; }
    public bool identified { get; set; }
    public int ilvl { get; set; }
    public PoeItemsProperty1[] properties { get; set; }
    public string[] implicitMods { get; set; }
    public string[] explicitMods { get; set; }
    public string descrText { get; set; }
    public string[] flavourText { get; set; }
    public int frameType { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public string inventoryId { get; set; }
    public string type { get; set; }
    public bool corrupted { get; set; }
    public PoePassivesRequirement[] requirements { get; set; }
    public int jewelRadius { get; set; }
    public string[] enchantMods { get; set; }

    public PoeItem ToPoeItem()
    {
      var _requirements = requirements ?? new PoePassivesRequirement[] { };
      var poeItemReqirements =
        _requirements
        .Select(r =>
            new PoeItemsRequirement() { name = r.name, values = r.values, displayMode = r.displayMode, suffix = "" })
        .ToArray();
      var item = new PoeItem()
      {
        verified = verified,
        w = w,
        h = h,
        icon = icon,
        league = league,
        id = id,
        name = name,
        typeLine = typeLine,
        identified = identified,
        corrupted = corrupted,
        ilvl = ilvl,
        properties = properties,
        requirements = poeItemReqirements,
        implicitMods = implicitMods,
        explicitMods = explicitMods,
        frameType = frameType,
        x = x,
        y = y,
        inventoryId = inventoryId,
        socketedItems = new PoeItemsSocketeditem[] { },
        craftedMods = new string[] { },
        enchantMods = enchantMods,
        flavourText = flavourText,
        descrText = descrText,
        utilityMods = new string[] { }
      };
      return item;
    }
  }

  //   public class PoePassivesProperty1
  //   {
  //     public string name { get; set; }
  //     public object[][] values { get; set; }
  //     public int displayMode { get; set; }
  //     public int type { get; set; }
  //   }

  public class PoePassivesRequirement
  {
    public string name { get; set; }
    public object[][] values { get; set; }
    public int displayMode { get; set; }
  }

}