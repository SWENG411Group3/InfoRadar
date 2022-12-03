using Microsoft.AspNetCore.Mvc;
using InformationRadarCore.Models;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Duende.IdentityServer.Extensions;
using InformationRadarCore.Services;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LighthouseController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment env;
        private readonly ConfigService config;
        private readonly IAuthService auth;
        private readonly IDynDb dynDb;

        public LighthouseController(ApplicationDbContext db, IWebHostEnvironment env, ConfigService config, IAuthService auth, IDynDb dynDb)
        {
            this.db = db;
            this.env = env;
            this.dynDb = dynDb;
            this.config = config;
            this.auth = auth;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] LighthouseSearchQuery query)
        {
            var tagNames = query.FixedTags;
            if (tagNames.Count() > 5)
            {
                return BadRequest(new
                {
                    Message = "No more than five tags are allowed in search",
                });
            }
            else if (!tagNames.Any() && string.IsNullOrWhiteSpace(query.Search))
            {
                return BadRequest(new
                { 
                    Message = "Either tags or search query required"
                });
            }

            var tags = new List<Tag>();
            if (tagNames.Any())
            {
                tags = await db.Tags.Where(t => tagNames.Contains(t.TagName)).ToListAsync();

                var invalidTags = tagNames.Except(tags.Select(e => e.TagName)).ToList();
                if (invalidTags.Any())
                {
                    return BadRequest(new
                    {
                        Message = "Nonexistant or invalid tags were supplied in the tags filter",
                        Tags = invalidTags,
                    });
                }
            }

            var lighthouses = await db.Lighthouses
                .OrderByDescending(e => e.Id)
                .Where(e =>
                    (string.IsNullOrEmpty(query.Search) || e.Title.Contains(query.Search)) &&
                    (!tags.Any() || (
                        e.Tags.Contains(tags[0])
                        && (tags.Count() < 2 || e.Tags.Contains(tags[1]))
                        && (tags.Count() < 3 || e.Tags.Contains(tags[2]))
                        && (tags.Count() < 4 || e.Tags.Contains(tags[3]))
                        && (tags.Count() < 5 || e.Tags.Contains(tags[4]))
                    ))
                )
                .Select(e => new LighthousePreview()
                {
                    Id = e.Id,
                    Title = e.Title,
                    InternalName = e.InternalName,
                    Description = e.Description,
                    HasError = e.HasError,
                    Enabled = e.Enabled,
                    Running = e.Running,
                    SearchRunning = e.SearchRunning,
                    Subscribed = e.Recipients.AsEnumerable().Any(user => user.Id == User.GetDisplayName()),
                    Thumbnail = e.Thumbnail,
                })
                .Take(500)
                .ToListAsync();

            return Ok(lighthouses);
        }

        [HttpGet]
        public async Task<CursorResponse<LighthousePreview, int?>> Get([FromQuery] PaginatorQuery<int?> query)
        {
            var lighthouses = await db.Lighthouses
                .OrderByDescending(e => e.Id)
                .Include(e => e.Tags)
                .Where(e => !query.Cursor.HasValue || e.Id < query.Cursor.Value)
                .Select(e => new LighthousePreview()
                {
                    Id = e.Id,
                    Title = e.Title,
                    InternalName = e.InternalName,
                    Description = e.Description,
                    HasError = e.HasError,
                    Enabled = e.Enabled,
                    Running = e.Running,
                    SearchRunning = e.SearchRunning,
                    Subscribed = e.Recipients.Any(user => user.Id == User.GetDisplayName()),
                    Thumbnail = e.Thumbnail,
                })
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = lighthouses.LastOrDefault()?.Id;
            return new CursorResponse<LighthousePreview, int?>()
            {
                Entries = lighthouses,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Lighthouses.AnyAsync(e => e.Id < cursor.Value)),
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var lighthouse = await db.Lighthouses
                .Select(e => new
                { 
                    e.Id, e.InternalName, e.Created,
                    e.Title, e.Description,
                    e.Running, e.SearchRunning,
                    e.Frequency, e.MessengerFrequency,
                    e.LastSentMessage, e.LastUpdated, e.LastVisitorRun,
                    e.LatestLog, e.HasError,
                    e.Thumbnail,
                    Tags = e.Tags.Select(e => new 
                        {
                            Id = e.TagId,
                            Value = e.TagName,
                        }).ToList(),
                    Sites = e.Sites.Select(e => new 
                        { 
                            e.Id,
                            Value = e.Url,
                        }).ToList(),
                    Queries = e.GoogleQueries.Select(e => new 
                        { 
                            e.Id, 
                            Value = e.Query, 
                        }).ToList(),
                    Subscribers = e.Recipients.Count(),
                    Subscribed = e.Recipients.Any(user => user.Id == User.GetDisplayName()),

                })
                .FirstOrDefaultAsync(e => e.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with ID {id}"
                });
            }

            return Ok(lighthouse);
        }

        [HttpPatch("{id}")]
        async public Task<IActionResult> Patch(int id, [FromBody] PatchLighthousePayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var lighthouse = await db.Lighthouses.FirstOrDefaultAsync(e => e.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new
                { 
                    Message = $"Lighthouse with ID \"{id}\" not found"
                });
            }

            var invalid = body.HasInvalidTag();
            if (!string.IsNullOrEmpty(invalid))
            {
                return BadRequest(new
                {
                    Message = $"Invalid tag: \"{invalid}\""
                });
            }

            body.MapLighthouse(lighthouse);

            await body.EnsureTags(db, lighthouse);
            await db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Create a custom lighthouse with a custom script
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("NewCustom")]
        [RequestFormLimits(ValueCountLimit = int.MaxValue)]
        async public Task<IActionResult> NewLighthouse([FromBody] CreateLighthousePayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            // Data validation
            if (await db.Lighthouses.AnyAsync(e => e.InternalName == body.InternalName))
            {
                return BadRequest(new
                {
                    Message = $"Lighthouse with internal name \"{body.InternalName}\" already exists",
                });
            }

            IDictionary<string, ILighthouseColumnType> columns;
            try
            {
                columns = LighthouseColumnUtil.ProcessRawCols(body.Types);
            }
            catch (LighthouseColumnException e)
            {
                return BadRequest(new
                {
                    Message = e.Message,
                });
            }

            var invalid = body.HasInvalidTag();
            if (!string.IsNullOrEmpty(invalid))
            {
                return BadRequest(new
                {
                    Message = $"Invalid tag: \"{invalid}\""
                });
            }

            if (!body.ValidateImg(env, config))
            {
                return NotFound(new
                {
                    Message = $"Thumbnail {body.Thumbnail} not found",
                });
            }
            
            // Data insertion
            await db.Database.BeginTransactionAsync();

            var lighthouseResult = await db.Lighthouses.AddAsync(new Lighthouse()
            {
                Created = DateTime.UtcNow,
                InternalName = body.InternalName,
                Title = body.Title,
                Description = body.Description,
                Enabled = body.Enabled ?? false,
                Frequency = body.Frequency,
                MessengerFrequency = body.MessengerFrequency,
                Thumbnail = body.Thumbnail,
            });

            await body.EnsureTags(db, lighthouseResult.Entity);
            await dynDb.CreateLighthouseTable(body.InternalName, columns);

            // File creation
            var path = config.CustomScriptPath(lighthouseResult.Entity);
            await System.IO.File.WriteAllTextAsync(path, body.Code);


            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            return Ok(new
            {
                lighthouseResult.Entity.Id,
                lighthouseResult.Entity.InternalName,
                lighthouseResult.Entity.Created,
            });
        }

        [HttpPost("New")]
        public async Task<IActionResult> TemplateLighthouse([FromBody] TemplateLighthousePayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var template = await db.Templates.SingleOrDefaultAsync(e => e.Id == body.TemplateId);
            if (template == null)
            {
                return NotFound(new
                {
                    Message = $"Template with id {body.TemplateId} not found",
                });
            }

            if (await db.Lighthouses.AnyAsync(e => e.InternalName == body.InternalName))
            {
                return BadRequest(new
                {
                    Message = $"Lighthouse with internal name \"{body.InternalName}\" already exists",
                });
            }

            var invalidTag = body.HasInvalidTag();
            if (!string.IsNullOrEmpty(invalidTag))
            {
                return BadRequest(new
                {
                    Message = $"Invalid tag: \"{invalidTag}\""
                });
            }

            if (!body.ValidateImg(env, config))
            {
                return NotFound(new
                {
                    Message = $"Thumbnail {body.Thumbnail} not found",
                });
            }

            var invalidParam = body.FindInvalidParam(await db.TemplateFields
                .Where(f => f.TemplateId == body.TemplateId)
                .ToListAsync());
            if (!string.IsNullOrEmpty(invalidParam))
            {
                return BadRequest(new
                {
                    Message = $"Field {invalidParam} is missing or invalid"
                });
            }

            // Data insertion
            await db.Database.BeginTransactionAsync();

            var columnsList = await db.TemplateLighthouseColumns
                .Where(col => col.TemplateId == body.TemplateId)
                .ToListAsync();

            IDictionary<string, ILighthouseColumnType> columns;
            try
            {
                var rawDict = columnsList.ToDictionary(col => col.Name, col => col.DataType);
                columns = LighthouseColumnUtil.ProcessRawCols(rawDict);
            }
            catch (LighthouseColumnException e)
            {
                return StatusCode(500, new
                {
                    Message = $"Error parsing stored column settings for template \"{template.InternalName}\": {e.Message}",
                });
            }

            var lighthouseResult = await db.Lighthouses.AddAsync(new Lighthouse()
            {
                Created = DateTime.UtcNow,
                InternalName = body.InternalName,
                Title = body.Title,
                Enabled = body.Enabled ?? false,
                Frequency = body.Frequency,
                MessengerFrequency = body.MessengerFrequency,
                Thumbnail = body.Thumbnail,
            });

            await db.TemplateConfigurations.AddAsync(new TemplateConfiguration()
            { 
                Payload = body.Params.ToString(),
                Lighthouse = lighthouseResult.Entity,
                Template = template,
            });

            await body.EnsureTags(db, lighthouseResult.Entity);

            await dynDb.CreateLighthouseTable(body.InternalName, columns);

            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            return Ok(new
            {
                lighthouseResult.Entity.Id,
                lighthouseResult.Entity.InternalName,
                lighthouseResult.Entity.Created,
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLighthouse(int id)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var lighthouse = await db.Lighthouses
                .Include(l => l.TemplateConfig)
                .SingleOrDefaultAsync(l => l.Id == id);
            if (lighthouse == null)
            {
                return BadRequest(new
                {
                    Message = $"Lighthouse with ID \"{id}\" doesn't exist",
                });
            }

            string? current = null;
            if (lighthouse.TemplateConfig == null)
            {
                current = config.CustomScriptPath(lighthouse);
                var dest = Path.Combine(config.ResourceRoot, "Old", "Scripts", lighthouse.InternalName + "_old_" + Path.GetRandomFileName());
                using (Stream source = System.IO.File.Open(current, FileMode.Open))
                {
                    using (Stream destination = System.IO.File.Create(dest))
                    {
                        await source.CopyToAsync(destination);
                    }
                }
            }

            await db.Database.BeginTransactionAsync();

            db.Lighthouses.Remove(lighthouse);
            await dynDb.DropLighthouseTable(lighthouse);

            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            if (current != null)
            { 
                System.IO.File.Delete(current);
            }

            return Ok();
        }

        [HttpPost("{id}/Tags")]
        public async Task<IActionResult> AddLighthouseTags(int id, [FromBody] TaggedPayload tags)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var lighthouse = await db.Lighthouses.SingleOrDefaultAsync(l => l.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new 
                { 
                    Message = $"No lighthouse with id {id}"
                });
            }

            var invalidTag = tags.HasInvalidTag();
            if (!string.IsNullOrEmpty(invalidTag))
            {
                return BadRequest(new
                {
                    Message = $"Invalid tag: \"{invalidTag}\""
                });
            }

            await tags.EnsureTags(db, lighthouse);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{lighthouseId}/Tags/{tagId}")]
        public async Task<IActionResult> RemoveLighthouseTags(int lighthouseId, int tagId)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var lighthouse = await db.Lighthouses
                .Include(l => l.Tags)
                .SingleOrDefaultAsync(l => l.Id == lighthouseId);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with ID \"{lighthouseId}\" found",
                });
            }

            var tag = lighthouse.Tags.FirstOrDefault(tag => tag.TagId == tagId);
            if (tag == null)
            {
                return NotFound(new
                { 
                    Message = $"No tag with ID \"{tagId}\" found"
                });
            }

            lighthouse.Tags.Remove(tag);
            await db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id}/Subscription")]
        public async Task<IActionResult> Subscription(int id, [FromBody] SubscriptionPayload body)
        {
            var lighthouse = await db.Lighthouses
                .Include(l => l.Recipients)
                .SingleOrDefaultAsync(l => l.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with id {id}"
                });
            }

            var user = await auth.GetUser();
            if (user == null)
            {
                return StatusCode(500, new
                {
                    Message = "Couldn't load the logged in user"
                });
            }

            if (body.Subscribed)
            {
                lighthouse.Recipients.Add(user);
            }
            else if (lighthouse.Recipients.Contains(user))
            {
                lighthouse.Recipients.Remove(user);
            }

            await db.SaveChangesAsync();

            return Ok();
        }

    }
}
