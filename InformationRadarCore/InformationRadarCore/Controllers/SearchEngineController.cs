using InformationRadarCore.Models.Web;
using InformationRadarCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InformationRadarCore.Data;
using InformationRadarCore.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class SearchEngineController : Controller
    {
        private readonly ApplicationDbContext db;
        public SearchEngineController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet("Sites")]
        public async Task<CursorResponse<Site>> GetSites([FromQuery] SearchQueryPaginatorQuery query)
        {
            int? lh = query.Lighthouse;
            var entries = await db.Sites
                .OrderByDescending(e => e.Id)
                .Where(e => (!query.Cursor.HasValue || e.Id < query.Cursor.Value) &&
                    (!lh.HasValue || e.LighthouseId == lh.Value))
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = entries.LastOrDefault()?.Id;
            return new CursorResponse<Site>()
            {
                Entries = entries,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Sites
                    .AnyAsync(e => e.Id < cursor && (!lh.HasValue || e.LighthouseId == lh.Value)))
            };
        }

        [HttpPost("Sites")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSite([FromBody] SitePayload body)
        {
            var lighthouse = await db.Lighthouses.SingleOrDefaultAsync(l => l.Id == body.Lighthouse);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with id \"{body.Lighthouse}\""
                });
            }

            var cannonical = body.Site.ToString();

            if (await db.Sites.AnyAsync(q => q.LighthouseId == body.Lighthouse && q.Url == cannonical))
            {
                return NotFound(new
                {
                    Message = "Site already inserted"
                });
            }

            db.Sites.Add(new Site()
            {
                Lighthouse = lighthouse,
                Url = cannonical,
            });
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Sites/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveSite(int id)
        {
            var site = await db.Sites.SingleOrDefaultAsync(q => q.Id == id);
            if (site == null)
            {
                return NotFound(new
                {
                    Message = $"No site with ID {id} found"
                });
            }

            db.Sites.Remove(site);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("Queries")]
        public async Task<CursorResponse<GoogleQuery>> GetQueries([FromQuery] SearchQueryPaginatorQuery query)
        {
            int? lh = query.Lighthouse;
            var entries = await db.GoogleQueries
                .OrderByDescending(e => e.Id)
                .Where(e => (!query.Cursor.HasValue || e.Id < query.Cursor.Value) && 
                    (!lh.HasValue || e.LighthouseId == lh.Value))
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = entries.LastOrDefault()?.Id;
            return new CursorResponse<GoogleQuery>()
            { 
                Entries = entries,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.GoogleQueries
                    .AnyAsync(e => e.Id < cursor && (!lh.HasValue || e.LighthouseId == lh.Value)))
            };
        }

        [HttpPost("Queries")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuery([FromBody] SearchQueryPayload body)
        {
            var lighthouse = await db.Lighthouses.SingleOrDefaultAsync(l => l.Id == body.Lighthouse);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with id \"{body.Lighthouse}\""
                });
            }

            string query = body.NormalizedQuery;
            if (await db.GoogleQueries.AnyAsync(q => q.LighthouseId == body.Lighthouse && q.Query == query))
            {
                return NotFound(new
                {
                    Message = "Query already exists"
                });
            }

            db.GoogleQueries.Add(new GoogleQuery
            {
                Lighthouse = lighthouse,
                Query = query,
            });
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("Queries/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveQuery(int id)
        {
            var query = await db.GoogleQueries.SingleOrDefaultAsync(q => q.Id == id);
            if (query == null)
            {
                return NotFound(new
                {
                    Message = $"No query with ID {id} found"
                });
            }

            db.GoogleQueries.Remove(query);
            await db.SaveChangesAsync();

            return Ok();
        }

    }
}
