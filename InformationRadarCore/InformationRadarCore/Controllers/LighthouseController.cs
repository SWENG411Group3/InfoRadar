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
    public class LighthouseResponse
    {
        public string Title { get; set; }
        public bool Active { get; set; }
        public bool Subscribed { get; set; }
        public bool Enabled { get; set; }
    }

    public class LighthousePageQuery
    {
        public int? PageSize { get; set; }
        public int? Cursor { get; set; }
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
        public LighthouseSize BaseSize { get; set; }


    }

    public class PatchLighthousePayload
    {
        public bool? Enabled { get; set; }
        public ulong? Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        public string Title { get; set; }
        public LighthouseSize? BaseSize { get; set; }
    }


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
                    Subscribed = e.Recipients.Any(user => user.Id == User.GetDisplayName()),
                    Recipients = e.Recipients.Count(),
                    Sites = e.Sites.Count(),
                    
                })
                .Take(size).ToListAsync();
        }

        // GET api/<LighthouseController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}")]
        async public Task<IActionResult> Patch(int id, [FromBody] PatchLighthousePayload body)
        {
            if (User.Identity == null)
            {
                return Unauthorized();
            }

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

            if (body.BaseSize.HasValue)
            {
                lighthouse.BaseSize = body.BaseSize.Value;
            }

            await db.SaveChangesAsync();

            return Ok();
        }

        // POST api/<LighthouseController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<LighthouseController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LighthouseController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
