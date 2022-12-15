using InformationRadarCore.Models.Web;
using InformationRadarCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InformationRadarCore.Data;
using InformationRadarCore.Services;
using Microsoft.EntityFrameworkCore;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class SearchEngineController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IAuthService auth;

        public SearchEngineController(ApplicationDbContext db, IAuthService auth)
        {
            this.db = db;
            this.auth = auth;
        }

        [HttpGet("Sites")]
        public async Task<CursorResponse<Site, int?>> GetSites([FromQuery] SearchQueryPaginatorQuery query)
        {
            int? lh = query.Lighthouse;
            var entries = await db.Sites
                .OrderByDescending(e => e.Id)
                .Where(e => (!query.Cursor.HasValue || e.Id < query.Cursor.Value) &&
                    (!lh.HasValue || e.LighthouseId == lh.Value))
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = entries.LastOrDefault()?.Id;
            return new CursorResponse<Site, int?>()
            {
                Entries = entries,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Sites
                    .AnyAsync(e => e.Id < cursor && (!lh.HasValue || e.LighthouseId == lh.Value)))
            };
        }

        [HttpPost("Sites")]
        public async Task<IActionResult> AddSite([FromBody] SitePayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var lighthouse = await db.Lighthouses.SingleOrDefaultAsync(l => l.Id == body.Lighthouse);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with id \"{body.Lighthouse}\""
                });
            }

            if (body.Site == null)
            {
                return BadRequest(new
                { 
                    Message = "Could not parse site parameter in request body"
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

            var siteResult = await db.Sites.AddAsync(new Site()
            {
                Lighthouse = lighthouse,
                Url = cannonical,
            });
            await db.SaveChangesAsync();

            return Ok(new
            {
                siteResult.Entity.Id,
            });
        }

        [HttpDelete("Sites/{id}")]
        public async Task<IActionResult> RemoveSite(int id)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

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
        public async Task<CursorResponse<GoogleQuery, int?>> GetQueries([FromQuery] SearchQueryPaginatorQuery query)
        {
            int? lh = query.Lighthouse;
            var entries = await db.GoogleQueries
                .OrderByDescending(e => e.Id)
                .Where(e => (!query.Cursor.HasValue || e.Id < query.Cursor.Value) && 
                    (!lh.HasValue || e.LighthouseId == lh.Value))
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = entries.LastOrDefault()?.Id;
            return new CursorResponse<GoogleQuery, int?>()
            { 
                Entries = entries,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.GoogleQueries
                    .AnyAsync(e => e.Id < cursor && (!lh.HasValue || e.LighthouseId == lh.Value)))
            };
        }

        [HttpPost("Queries")]
        public async Task<IActionResult> AddQuery([FromBody] SearchQueryPayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

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

            var queryResult = await db.GoogleQueries.AddAsync(new GoogleQuery
            {
                Lighthouse = lighthouse,
                Query = query,
                NumResults = body.NumResults,
            });
            await db.SaveChangesAsync();

            return Ok(new
            { 
                queryResult.Entity.Id,
            });
        }

        [HttpDelete("Queries/{id}")]
        public async Task<IActionResult> RemoveQuery(int id)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

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

        [HttpPatch("Queries/{id}/Results")]
        public async Task<IActionResult> ChangeQueryNumResults(int id, [FromBody] PatchSearchQuery body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var query = await db.GoogleQueries.SingleOrDefaultAsync(q => q.Id == id);
            if (query == null)
            {
                return NotFound(new
                {
                    Message = $"No query with ID {id} found"
                });
            }

            query.NumResults = body.NumResults;
            await db.SaveChangesAsync();

            return Ok();
        }

    }
}
