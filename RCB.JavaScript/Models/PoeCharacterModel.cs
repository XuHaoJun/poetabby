using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RCB.JavaScript.Models.Utils;

namespace RCB.JavaScript.Models
{
  public class PoeDepth
  {
    public int Default { get; set; }
    public int Solo { get; set; }
  }

  public class PoeChallenges
  {
    public int Total { get; set; }
  }
  public class PoeTwitch
  {
    public string Name { get; set; }
  }

  public class PoeAccount
  {
    public string Name { get; set; }
    public string Realm { get; set; }
    public PoeChallenges Challenges { get; set; }
    public PoeTwitch Twitch { get; set; }
  }

  public class PoeCharacterModel
  {
    [Key]
    public string CharacterId { get; set; }
    [Required]
    public string LeagueName { get; set; }
    [Required]
    public string CharacterName { get; set; }
    [Required]
    public string AccountName { get; set; }
    [Column(TypeName = "jsonb")]
    public PoeAccount Account { get; set; }
    [Required]
    public int Level { get; set; }
    [Required]
    public long LifeUnreserved { get; set; }
    [Required]
    public long EnergyShield { get; set; }
    [Required]
    public string Class { get; set; }
    public bool Dead { get; set; }
    public bool Online { get; set; }
    public int Rank { get; set; }
    [Column(TypeName = "jsonb")]
    public PoeDepth Depth { get; set; }
    [Required]
    public long Experience { get; set; }
    [Required]
    public string PobCode { get; set; }
    [Required]
    [Column(TypeName = "jsonb")]
    public PathOfBuilding Pob { get; set; }
    [Required]
    [Column(TypeName = "jsonb")]
    public List<PoeItem> Items { get; set; }
    [Required]
    public System.DateTime UpdatedAt { get; set; }
  }
}
