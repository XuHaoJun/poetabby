using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RCB.JavaScript.Models.Utils;
using DateTime = System.DateTime;

namespace RCB.JavaScript.Models
{

  public class PoeLeagueRules
  {
    string Id { get; set; }
    string Name { get; set; }
    public string Description { get; set; }
  }

  public class PoeLeagueModel
  {
    [Key]
    public string LeagueId { get; set; }

    [Required]
    public string Realm { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime RegisterAt { get; set; }

    [Required]
    public string Url { get; set; }

    [Required]
    public DateTime StartAt { get; set; }

    public DateTime? EndAt { get; set; }

    [Required]
    public bool DelveEvent { get; set; }

    [Column(TypeName = "jsonb")]
    public List<PoeLeagueRules> Rules { get; set; }
  }
}
