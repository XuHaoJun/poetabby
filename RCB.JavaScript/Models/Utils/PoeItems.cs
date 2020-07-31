namespace RCB.JavaScript.Models.Utils
{
    public class PoeItems
    {
        public System.Collections.Generic.List<PoeItem> items { get; set; }
        public PoeItemsCharacter character { get; set; }
    }

    public class PoeItemsCharacter
    {
        public string name { get; set; }
        public string league { get; set; }
        public int classId { get; set; }
        public int ascendancyClass { get; set; }
        public string _class { get; set; }
        public int level { get; set; }
        public long experience { get; set; }
        public bool lastActive { get; set; }
    }

    public class PoeItem
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string icon { get; set; }
        public string league { get; set; }
        public string id { get; set; }
        public PoeItemsSocket[] sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public bool corrupted { get; set; }
        public int ilvl { get; set; }
        public PoeItemsProperty1[] properties { get; set; }
        public PoeItemsRequirement[] requirements { get; set; }
        public string[] implicitMods { get; set; }
        public string[] explicitMods { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public PoeItemsSocketeditem[] socketedItems { get; set; }
        public string[] craftedMods { get; set; }
        public string[] enchantMods { get; set; }
        public string[] flavourText { get; set; }
        public string descrText { get; set; }
        public string[] utilityMods { get; set; }
    }

    public class PoeItemsSocket
    {
        public int group { get; set; }
        public string attr { get; set; }
        public string sColour { get; set; }
    }

    public class PoeItemsProperty1
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
        public int type { get; set; }
    }

    public class PoeItemsRequirement
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
        public string suffix { get; set; }
    }

    public class PoeItemsSocketeditem
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string icon { get; set; }
        public bool support { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public int ilvl { get; set; }
        public Property3[] properties { get; set; }
        public Requirement1[] requirements { get; set; }
        public Additionalproperty[] additionalProperties { get; set; }
        public string secDescrText { get; set; }
        public string[] explicitMods { get; set; }
        public string descrText { get; set; }
        public int frameType { get; set; }
        public int socket { get; set; }
        public string colour { get; set; }
        public Hybrid hybrid { get; set; }
        public bool abyssJewel { get; set; }
        public Nextlevelrequirement[] nextLevelRequirements { get; set; }
        public bool corrupted { get; set; }
    }

    public class Hybrid
    {
        public string baseTypeName { get; set; }
        public Property2[] properties { get; set; }
        public string[] explicitMods { get; set; }
        public string secDescrText { get; set; }
    }

    public class Property2
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
    }

    public class Property3
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
        public int type { get; set; }
    }

    public class Requirement1
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
    }

    public class Additionalproperty
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
        public float progress { get; set; }
        public int type { get; set; }
    }

    public class Nextlevelrequirement
    {
        public string name { get; set; }
        public object[][] values { get; set; }
        public int displayMode { get; set; }
    }
}