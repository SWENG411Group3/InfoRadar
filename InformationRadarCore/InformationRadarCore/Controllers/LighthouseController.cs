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
        private readonly DynamicDb dynamicDb;
        private readonly IWebHostEnvironment env;
        private readonly ConfigService config;

        public LighthouseController(ApplicationDbContext db, IWebHostEnvironment env, ConfigService configuration)
        {
            this.db = db;
            this.env = env;
            dynamicDb = new DynamicDb(db);
            config = configuration;
        }

        [HttpGet]
        public async Task<CursorResponse<LighthousePreview>> Get([FromQuery] LighthousesPaginatorQuery query)
        {
            var lighthouses = await db.Lighthouses
                .OrderByDescending(e => e.Id)
                .Where(e => 
                    (!query.Cursor.HasValue || e.Id < query.Cursor.Value) && 
                    (query.Tags == null || query.Tags.All(t => e.Tags.Any(tag => tag.TagName == t)))
                )
                .Select(e => new LighthousePreview()
                {
                    Id = e.Id,
                    Title = e.Title,
                    InternalName = e.InternalName,
                    Description = e.Description,
                    Enabled = e.Enabled,
                    Running = e.Running,
                    SearchRunning = e.SearchRunning,
                    Subscribed = e.Recipients.Any(user => user.Id == User.GetDisplayName()),
                    Thumbnail = e.Thumbnail,
                })
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = lighthouses.LastOrDefault()?.Id;
            return new CursorResponse<LighthousePreview>()
            {
                Entries = lighthouses,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Lighthouses.AnyAsync(e => 
                    e.Id < cursor.Value && (query.Tags == null || query.Tags.All(t => e.Tags.Any(tag => tag.TagName == t)))
                )),
            };
        }

        [HttpGet("{id}")]
        public Task<Lighthouse?> Get(int id)
        {
            return db.Lighthouses.FirstOrDefaultAsync(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}")]
        async public Task<IActionResult> Patch(int id, [FromBody] PatchLighthousePayload body)
        {
            var lighthouse = await db.Lighthouses.FirstOrDefaultAsync(e => e.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new
                { 
                    Message = $"Lighthouse with ID \"{id}\" not found"
                });
            }

            body.MapLighthouse(lighthouse);
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
        [Authorize(Roles = "Admin")]
        async public Task<IActionResult> NewLighthouse([FromBody] CreateLighthousePayload body)
        {
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

            if (!body.ValidateImg())
            {
                return BadRequest(new
                {
                    Message = CreateLighthousePayload.INVALID_IMG_MSG
                });
            }

            // Data insertion
            await db.Database.BeginTransactionAsync();
            var thumbnailFilename = body.RandomFilename();

            var lighthouseResult = await db.Lighthouses.AddAsync(new Lighthouse()
            {
                Created = DateTime.UtcNow,
                InternalName = body.InternalName,
                Title = body.Title,
                Enabled = body.Enabled ?? false,
                Frequency = body.Frequency,
                MessengerFrequency = body.MessengerFrequency,
                Thumbnail = thumbnailFilename,
            });

            await body.EnsureTags(db, lighthouseResult.Entity);
            await dynamicDb.CreateLighthouseTable(body.InternalName, columns);

            // File creation
            var path = config.CustomScriptPath(lighthouseResult.Entity);
            await System.IO.File.WriteAllTextAsync(path, body.Code);

            if (thumbnailFilename != null)
            {
                await body.UploadImage(thumbnailFilename, env, config);
            }

            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            return Ok(lighthouseResult.Entity);
        }

        [HttpPost("New")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TemplateLighthouse([FromBody] TemplateLighthousePayload body)
        {
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

            if (!body.ValidateImg())
            {
                return BadRequest(new
                {
                    Message = CreateLighthousePayload.INVALID_IMG_MSG
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
            var thumbnailFilename = body.RandomFilename();

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
                Thumbnail = thumbnailFilename,
            });

            await db.TemplateConfigurations.AddAsync(new TemplateConfiguration()
            { 
                Payload = body.Params.ToString(),
                Lighthouse = lighthouseResult.Entity,
                Template = template,
            });

            if (thumbnailFilename != null)
            {
                await body.UploadImage(thumbnailFilename, env, config);
            }

            await body.EnsureTags(db, lighthouseResult.Entity);

            await dynamicDb.CreateLighthouseTable(body.InternalName, columns);

            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            return Ok(lighthouseResult.Entity);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLighthouse(int id)
        {
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
            await dynamicDb.DropLighthouseTable(lighthouse);

            await db.SaveChangesAsync();
            await db.Database.CommitTransactionAsync();

            if (current != null)
            { 
                System.IO.File.Delete(current);
            }

            return Ok();
        }

        [HttpPost("{id}/Tags")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddLighthouseTags(int id, [FromBody] TaggedPayload tags)
        {
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
            return Ok();
        }

        [HttpDelete("{lighthouseId}/Tags/{tagId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveLighthouseTags(int lighthouseId, int tagId)
        {
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
    }
}
