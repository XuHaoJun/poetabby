using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using RCB.JavaScript.Models;
using RCB.JavaScript.Models.Utils;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace RCB.JavaScript.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class CharactersController : ControllerBase
  {
    private readonly PoeDbContext PoeContext;

    public CharactersController(PoeDbContext context)
    {
      PoeContext = context;
    }

    [HttpGet("{accountName}/{characterName}")]
    public async Task<ActionResult<PoeCharacterModel>> GetCharacterByName(string accountName, string characterName)
    {
      var pchar = await PoeContext.Characters.SingleOrDefaultAsync(c => c.AccountName == accountName && c.CharacterName == characterName);
      if (pchar == null)
      {
        return NotFound();
      }
      else
      {
        Response.Headers.Add("Cache-Control", "public, max-age=60");
        pchar.CountAnalysis = null;
        return pchar;
      }
    }

    [HttpGet("{characterId}")]
    public async Task<ActionResult<PoeCharacterModel>> GetCharacterById(string characterId)
    {
      var pchar = await PoeContext.Characters.SingleOrDefaultAsync(c => c.CharacterId == characterId);
      if (pchar == null)
      {
        return NotFound();
      }
      else
      {
        Response.Headers.Add("Cache-Control", "public, max-age=60");
        pchar.CountAnalysis = null;
        return pchar;
      }
    }
  }
}