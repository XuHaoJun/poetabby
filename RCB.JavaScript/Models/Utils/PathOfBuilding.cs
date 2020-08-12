using System.Xml.Serialization;
using System.Text.Json.Serialization;


namespace RCB.JavaScript.Models.Utils
{

  // 注意: 產生的程式碼可能至少需要 .NET Framework 4.5 或 .NET Core/Standard 2.0。
  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class PathOfBuilding
  {

    private PathOfBuildingBuild buildField;

    private object importField;

    private PathOfBuildingCalcs calcsField;

    private PathOfBuildingSkills skillsField;

    private PathOfBuildingTree treeField;

    private string notesField;

    private PathOfBuildingTreeView treeViewField;

    private PathOfBuildingItems itemsField;

    private PathOfBuildingInput[] configField;

    /// <remarks/>
    public PathOfBuildingBuild Build
    {
      get
      {
        return this.buildField;
      }
      set
      {
        this.buildField = value;
      }
    }

    /// <remarks/>
    public object Import
    {
      get
      {
        return this.importField;
      }
      set
      {
        this.importField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingCalcs Calcs
    {
      get
      {
        return this.calcsField;
      }
      set
      {
        this.calcsField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingSkills Skills
    {
      get
      {
        return this.skillsField;
      }
      set
      {
        this.skillsField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingTree Tree
    {
      get
      {
        return this.treeField;
      }
      set
      {
        this.treeField = value;
      }
    }

    /// <remarks/>
    public string Notes
    {
      get
      {
        return this.notesField;
      }
      set
      {
        this.notesField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingTreeView TreeView
    {
      get
      {
        return this.treeViewField;
      }
      set
      {
        this.treeViewField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingItems Items
    {
      get
      {
        return this.itemsField;
      }
      set
      {
        this.itemsField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Input", IsNullable = false)]
    public PathOfBuildingInput[] Config
    {
      get
      {
        return this.configField;
      }
      set
      {
        this.configField = value;
      }
    }

    public long GetStat(string name)
    {
      foreach (var stat in Build.PlayerStat)
      {
        if (stat.stat == name)
        {
          return (long)stat.value;
        }
      }
      return 0;
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingBuild
  {

    private PathOfBuildingBuildPlayerStat[] playerStatField;

    private byte levelField;

    private string targetVersionField;

    private string banditNormalField;

    private string banditField;

    private string banditMercilessField;

    private string classNameField;

    private string ascendClassNameField;

    private byte mainSocketGroupField;

    private string viewModeField;

    private string banditCruelField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("PlayerStat")]
    public PathOfBuildingBuildPlayerStat[] PlayerStat
    {
      get
      {
        return this.playerStatField;
      }
      set
      {
        this.playerStatField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte level
    {
      get
      {
        return this.levelField;
      }
      set
      {
        this.levelField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string targetVersion
    {
      get
      {
        return this.targetVersionField;
      }
      set
      {
        this.targetVersionField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string banditNormal
    {
      get
      {
        return this.banditNormalField;
      }
      set
      {
        this.banditNormalField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string bandit
    {
      get
      {
        return this.banditField;
      }
      set
      {
        this.banditField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string banditMerciless
    {
      get
      {
        return this.banditMercilessField;
      }
      set
      {
        this.banditMercilessField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string className
    {
      get
      {
        return this.classNameField;
      }
      set
      {
        this.classNameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string ascendClassName
    {
      get
      {
        return this.ascendClassNameField;
      }
      set
      {
        this.ascendClassNameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte mainSocketGroup
    {
      get
      {
        return this.mainSocketGroupField;
      }
      set
      {
        this.mainSocketGroupField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string viewMode
    {
      get
      {
        return this.viewModeField;
      }
      set
      {
        this.viewModeField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string banditCruel
    {
      get
      {
        return this.banditCruelField;
      }
      set
      {
        this.banditCruelField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingBuildPlayerStat
  {

    private string statField;

    private decimal valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string stat
    {
      get
      {
        return this.statField;
      }
      set
      {
        this.statField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal value
    {
      get
      {
        return this.valueField;
      }
      set
      {
        this.valueField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingCalcs
  {

    private PathOfBuildingCalcsInput[] inputField;

    private PathOfBuildingCalcsSection[] sectionField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Input")]
    public PathOfBuildingCalcsInput[] Input
    {
      get
      {
        return this.inputField;
      }
      set
      {
        this.inputField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Section")]
    public PathOfBuildingCalcsSection[] Section
    {
      get
      {
        return this.sectionField;
      }
      set
      {
        this.sectionField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingCalcsInput
  {

    private string nameField;

    private byte numberField;

    private bool numberFieldSpecified;

    private string stringField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte number
    {
      get
      {
        return this.numberField;
      }
      set
      {
        this.numberField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool numberSpecified
    {
      get
      {
        return this.numberFieldSpecified;
      }
      set
      {
        this.numberFieldSpecified = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string @string
    {
      get
      {
        return this.stringField;
      }
      set
      {
        this.stringField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingCalcsSection
  {

    private bool collapsedField;

    private string idField;

    [JsonIgnore]
    [XmlAttributeAttribute("collapsed")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string collapsedString
    {
      get; set;
    }

    [XmlIgnore]
    private bool collapsedSet = false;

    [XmlIgnore]
    public bool collapsed
    {
      get
      {
        if (!collapsedSet)
        {
          if (!string.IsNullOrEmpty(collapsedString))
          {
            if (collapsedString.ToLower() == "true")
            {
              this.collapsedField = true;
            }
            else
            {
              this.collapsedField = false;
            }
            collapsedSet = true;
          }
        }
        return this.collapsedField;
      }
      set
      {
        this.collapsedField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingSkills
  {

    private PathOfBuildingSkillsSkill[] skillField;

    private string defaultGemQualityField;

    private string defaultGemLevelField;

    private string showSupportGemTypesField;

    private bool sortGemsByDPSField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Skill")]
    public PathOfBuildingSkillsSkill[] Skill
    {
      get
      {
        return this.skillField;
      }
      set
      {
        this.skillField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string defaultGemQuality
    {
      get
      {
        return this.defaultGemQualityField;
      }
      set
      {
        this.defaultGemQualityField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string defaultGemLevel
    {
      get
      {
        return this.defaultGemLevelField;
      }
      set
      {
        this.defaultGemLevelField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string showSupportGemTypes
    {
      get
      {
        return this.showSupportGemTypesField;
      }
      set
      {
        this.showSupportGemTypesField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool sortGemsByDPS
    {
      get
      {
        return this.sortGemsByDPSField;
      }
      set
      {
        this.sortGemsByDPSField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingSkillsSkill
  {

    private PathOfBuildingSkillsSkillGem[] gemField;

    private byte mainActiveSkillCalcsField;

    private string labelField;

    private bool enabledField;

    private string slotField;

    private byte mainActiveSkillField;

    private string sourceField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Gem")]
    public PathOfBuildingSkillsSkillGem[] Gem
    {
      get
      {
        return this.gemField;
      }
      set
      {
        this.gemField = value;
      }
    }

    [JsonIgnore]
    [XmlAttributeAttribute("mainActiveSkillCalcs")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string mainActiveSkillCalcsString
    {
      get; set;
    }

    [XmlIgnore]
    private bool mainActiveSkillCalcsSet = false;

    [XmlIgnore]
    public byte mainActiveSkillCalcs
    {
      get
      {
        if (!mainActiveSkillCalcsSet)
        {
          if (!string.IsNullOrEmpty(mainActiveSkillCalcsString))
          {
            if (mainActiveSkillCalcsString == "nil")
            {
              this.mainActiveSkillCalcsField = 1;
            }
            else
            {
              byte.TryParse(mainActiveSkillCalcsString, out this.mainActiveSkillCalcsField);
            }
          }
          mainActiveSkillCalcsSet = true;
        }
        return this.mainActiveSkillCalcsField;
      }
      set
      {
        this.mainActiveSkillCalcsField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string label
    {
      get
      {
        return this.labelField;
      }
      set
      {
        this.labelField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool enabled
    {
      get
      {
        return this.enabledField;
      }
      set
      {
        this.enabledField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string slot
    {
      get
      {
        return this.slotField;
      }
      set
      {
        this.slotField = value;
      }
    }

    [JsonIgnore]
    [XmlAttributeAttribute("mainActiveSkill")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string mainActiveSkillString
    {
      get; set;
    }

    [XmlIgnore]
    private bool mainActiveSkillSet = false;

    [XmlIgnore]
    public byte mainActiveSkill
    {
      get
      {
        if (!mainActiveSkillSet)
        {
          if (!string.IsNullOrEmpty(mainActiveSkillString))
          {
            if (mainActiveSkillString == "nil")
            {
              this.mainActiveSkillField = 1;
            }
            else
            {
              byte.TryParse(mainActiveSkillString, out this.mainActiveSkillField);
            }
          }
          mainActiveSkillSet = true;
        }
        return this.mainActiveSkillField;
      }
      set
      {
        this.mainActiveSkillField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string source
    {
      get
      {
        return this.sourceField;
      }
      set
      {
        this.sourceField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingSkillsSkillGem
  {

    private bool enableGlobal2Field;

    private byte qualityField;

    private byte levelField;

    private string gemIdField;

    private string skillIdField;

    private bool enableGlobal1Field;

    private bool enabledField;

    private string nameSpecField;

    private byte skillPartField;

    private bool skillPartFieldSpecified;

    private byte skillPartCalcsField;

    private bool skillPartCalcsFieldSpecified;

    [JsonIgnore]
    [XmlAttributeAttribute("enableGlobal2")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public string enableGlobal2String
    {
      get; set;
    }

    [XmlIgnore]
    private bool enableGlobal2Set = false;

    [XmlIgnore]
    public bool enableGlobal2
    {
      get
      {
        if (!enableGlobal2Set)
        {
          if (!string.IsNullOrEmpty(enableGlobal2String))
          {
            if (enableGlobal2String.ToLower() == "true")
            {
              this.enableGlobal2Field = true;
            }
            else
            {
              this.enableGlobal2Field = false;
            }
            enableGlobal2Set = true;
          }
        }
        return this.enableGlobal2Field;
      }
      set
      {
        this.enableGlobal2Field = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte quality
    {
      get
      {
        return this.qualityField;
      }
      set
      {
        this.qualityField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte level
    {
      get
      {
        return this.levelField;
      }
      set
      {
        this.levelField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string gemId
    {
      get
      {
        return this.gemIdField;
      }
      set
      {
        this.gemIdField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string skillId
    {
      get
      {
        return this.skillIdField;
      }
      set
      {
        this.skillIdField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool enableGlobal1
    {
      get
      {
        return this.enableGlobal1Field;
      }
      set
      {
        this.enableGlobal1Field = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool enabled
    {
      get
      {
        return this.enabledField;
      }
      set
      {
        this.enabledField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nameSpec
    {
      get
      {
        return this.nameSpecField;
      }
      set
      {
        this.nameSpecField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte skillPart
    {
      get
      {
        return this.skillPartField;
      }
      set
      {
        this.skillPartField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool skillPartSpecified
    {
      get
      {
        return this.skillPartFieldSpecified;
      }
      set
      {
        this.skillPartFieldSpecified = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte skillPartCalcs
    {
      get
      {
        return this.skillPartCalcsField;
      }
      set
      {
        this.skillPartCalcsField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool skillPartCalcsSpecified
    {
      get
      {
        return this.skillPartCalcsFieldSpecified;
      }
      set
      {
        this.skillPartCalcsFieldSpecified = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingTree
  {

    private PathOfBuildingTreeSpec specField;

    private byte activeSpecField;

    /// <remarks/>
    public PathOfBuildingTreeSpec Spec
    {
      get
      {
        return this.specField;
      }
      set
      {
        this.specField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte activeSpec
    {
      get
      {
        return this.activeSpecField;
      }
      set
      {
        this.activeSpecField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingTreeSpec
  {

    private string uRLField;

    private PathOfBuildingTreeSpecSocket[] socketsField;

    private byte ascendClassIdField;

    private string nodesField;

    private string treeVersionField;

    private byte classIdField;

    /// <remarks/>
    public string URL
    {
      get
      {
        return this.uRLField;
      }
      set
      {
        this.uRLField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Socket", IsNullable = false)]
    public PathOfBuildingTreeSpecSocket[] Sockets
    {
      get
      {
        return this.socketsField;
      }
      set
      {
        this.socketsField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte ascendClassId
    {
      get
      {
        return this.ascendClassIdField;
      }
      set
      {
        this.ascendClassIdField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string nodes
    {
      get
      {
        return this.nodesField;
      }
      set
      {
        this.nodesField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string treeVersion
    {
      get
      {
        return this.treeVersionField;
      }
      set
      {
        this.treeVersionField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte classId
    {
      get
      {
        return this.classIdField;
      }
      set
      {
        this.classIdField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingTreeSpecSocket
  {

    private ushort nodeIdField;

    private byte itemIdField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public ushort nodeId
    {
      get
      {
        return this.nodeIdField;
      }
      set
      {
        this.nodeIdField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte itemId
    {
      get
      {
        return this.itemIdField;
      }
      set
      {
        this.itemIdField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingTreeView
  {

    private string searchStrField;

    private byte zoomYField;

    private bool showHeatMapField;

    private byte zoomLevelField;

    private bool showStatDifferencesField;

    private byte zoomXField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string searchStr
    {
      get
      {
        return this.searchStrField;
      }
      set
      {
        this.searchStrField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte zoomY
    {
      get
      {
        return this.zoomYField;
      }
      set
      {
        this.zoomYField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool showHeatMap
    {
      get
      {
        return this.showHeatMapField;
      }
      set
      {
        this.showHeatMapField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte zoomLevel
    {
      get
      {
        return this.zoomLevelField;
      }
      set
      {
        this.zoomLevelField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool showStatDifferences
    {
      get
      {
        return this.showStatDifferencesField;
      }
      set
      {
        this.showStatDifferencesField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte zoomX
    {
      get
      {
        return this.zoomXField;
      }
      set
      {
        this.zoomXField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingItems
  {

    private PathOfBuildingItemsItem[] itemField;

    private PathOfBuildingItemsSlot[] slotField;

    private PathOfBuildingItemsItemSet itemSetField;

    private byte activeItemSetField;

    private bool useSecondWeaponSetField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Item")]
    public PathOfBuildingItemsItem[] Item
    {
      get
      {
        return this.itemField;
      }
      set
      {
        this.itemField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Slot")]
    public PathOfBuildingItemsSlot[] Slot
    {
      get
      {
        return this.slotField;
      }
      set
      {
        this.slotField = value;
      }
    }

    /// <remarks/>
    public PathOfBuildingItemsItemSet ItemSet
    {
      get
      {
        return this.itemSetField;
      }
      set
      {
        this.itemSetField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte activeItemSet
    {
      get
      {
        return this.activeItemSetField;
      }
      set
      {
        this.activeItemSetField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool useSecondWeaponSet
    {
      get
      {
        return this.useSecondWeaponSetField;
      }
      set
      {
        this.useSecondWeaponSetField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingItemsItem
  {

    private byte idField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
      get
      {
        return this.valueField;
      }
      set
      {
        this.valueField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingItemsSlot
  {

    private string nameField;

    private byte itemIdField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte itemId
    {
      get
      {
        return this.itemIdField;
      }
      set
      {
        this.itemIdField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingItemsItemSet
  {

    private PathOfBuildingItemsItemSetSlot[] slotField;

    private bool useSecondWeaponSetField;

    private byte idField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Slot")]
    public PathOfBuildingItemsItemSetSlot[] Slot
    {
      get
      {
        return this.slotField;
      }
      set
      {
        this.slotField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool useSecondWeaponSet
    {
      get
      {
        return this.useSecondWeaponSetField;
      }
      set
      {
        this.useSecondWeaponSetField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte id
    {
      get
      {
        return this.idField;
      }
      set
      {
        this.idField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingItemsItemSetSlot
  {

    private string nameField;

    private byte itemIdField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public byte itemId
    {
      get
      {
        return this.itemIdField;
      }
      set
      {
        this.itemIdField = value;
      }
    }
  }

  /// <remarks/>
  [System.SerializableAttribute()]
  [System.ComponentModel.DesignerCategoryAttribute("code")]
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  public partial class PathOfBuildingInput
  {

    private string nameField;

    private bool booleanField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public bool boolean
    {
      get
      {
        return this.booleanField;
      }
      set
      {
        this.booleanField = value;
      }
    }
  }
}