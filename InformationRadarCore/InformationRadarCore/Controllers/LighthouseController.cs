using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using InformationRadarCore.Models;
using InformationRadarCore.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer.Extensions;
using IdentityModel;
using System.Security.Claims;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

// @TODO implement user permission levels
namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LighthouseController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        public LighthouseController(ApplicationDbContext db)
        {
            this.db = db;
        }

        // GET: api/<LighthouseController>
        [HttpGet]
        public async Task<IEnumerable<object>> Get([FromQuery] LighthousePageQuery query)
        {
            var size = query.PageSize ?? 10;
            
            return await db.Lighthouses.OrderBy(e => e.Id)
                .Where(e => !query.Cursor.HasValue || e.Id < query.Cursor.Value)
                .Select(e => new
                {
                    Id = e.Id,
                    Title = e.Title,
                    Enabled = e.Enabled,
                    Subscribed = e.Recipients.Any(user => user.Id == User.GetDisplayName()),
                })
                .Take(size).ToListAsync();
        }

        // GET api/<LighthouseController>/5
        [HttpGet("{id}")]
        public Task<Lighthouse?> Get(int id)
        {
            return db.Lighthouses.FirstOrDefaultAsync(e => e.Id == id);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        async public Task<IActionResult> Patch(int id, [FromBody] PatchLighthousePayload body)
        {
            var lighthouse = await db.Lighthouses.FirstOrDefaultAsync(e => e.Id == id);
            if (lighthouse == null)
            {
                return NotFound();
            }

            if (body.Enabled.HasValue)
            {
                lighthouse.Enabled = body.Enabled.Value;
            }

            if (body.Title != null)
            {
                lighthouse.Title = body.Title;
            }

            if (body.Frequency.HasValue)
            {
                lighthouse.Frequency = body.Frequency.Value;
            }

            if (body.MessengerFrequency.HasValue)
            {
                lighthouse.MessengerFrequency = body.MessengerFrequency.Value;
            }

            await db.SaveChangesAsync();

            return Ok();
        }
    }

    public class LighthousePageQuery
    {
        public int? PageSize { get; set; }
        public int? Cursor { get; set; }
    }

    public enum LighthouseRecordType {
        Int,
        Long,
        Double,
        String,
    }

    public class CreateLighthousePayload
    {
        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z]*$")]
        public string InternalName { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; }
        public bool? Enabled { get; set; }
        public ulong? Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        [Required]
        public Dictionary<string, LighthouseRecordType> Types { get; set; }
    }

    public class PatchLighthousePayload
    {
        public bool? Enabled { get; set; }
        public ulong? Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        public string Title { get; set; }
    }
}
