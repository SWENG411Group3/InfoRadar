using InformationRadarCore.Data;
using InformationRadarCore.Models;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InformationRadarCore.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class TemplateController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ConfigService config;

        public TemplateController(ApplicationDbContext db, ConfigService configuration)
        {
            this.db = db;
            config = configuration;
        }

        [HttpGet]
        public async Task<CursorResponse<Template>> Index([FromQuery] PaginatorQuery query)
        {
            var entries = await db.Templates
                .OrderByDescending(e => e.Id)
                .Where(e => !query.Cursor.HasValue || e.Id < query.Cursor.Value)
                .Include(t => t.LighthouseColumns)
                .Include(t => t.TemplateFields)
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = entries.LastOrDefault()?.Id;
            return new CursorResponse<Template>()
            {
                Entries = entries,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Templates.AnyAsync(e => e.Id < cursor))
            };
        }

        [HttpGet("{id}")]
        public Task<Template?> Details(int id)
        {
            return db.Templates
                .Include(t => t.LighthouseColumns)
                .Include(t => t.TemplateFields)
                .FirstOrDefaultAsync(template => template.Id == id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] TemplatePayload body)
        {
            if (await db.Templates.AnyAsync(t => t.InternalName == body.InternalName))
            {
                return BadRequest(new
                {
                    Message = $"Template with name \"{body.InternalName}\" already exists"
                });
            }

            IDictionary<string, ILighthouseColumnType> columns;
            try
            {
                columns = LighthouseColumnUtil.ProcessRawCols(body.Columns);
            }
            catch (LighthouseColumnException e)
            {
                return BadRequest(new
                {
                    Message = e.Message,
                });
            }

            foreach (var col in body.Fields.Keys)
            {
                if (!Regex.IsMatch(col, @"^[a-zA-Z]\w{0,99}$"))
                {
                    return BadRequest(new
                    {
                        Message = $"Invalid template field \"{col}\".  Must be 100 chars or less and abide by Python identifier naming rules",
                    });
                }
            }

            var tmp = await db.Templates.AddAsync(new Template()
            {
                Title = body.Title,
                Description = body.Description,
                InternalName = body.InternalName,
            });

            var path = config.TemplateScriptPath(tmp.Entity);
            await System.IO.File.WriteAllTextAsync(path, body.Code);

            foreach (var (key, type) in columns)
            {
                await db.TemplateLighthouseColumns.AddAsync(new TemplateLighthouseColumn()
                {
                    Name = key,
                    DataType = type.Seralize(),
                    Template = tmp.Entity,
                });
            }

            foreach (var (key, data) in body.Fields)
            {
                await db.TemplateFields.AddAsync(new TemplateField()
                {
                    Name = key,
                    DataType = data.Type,
                    Description = data.Description,
                    Template = tmp.Entity,
                });
            }

            await db.SaveChangesAsync();

            return Ok(new
            {
                Id = tmp.Entity.Id,
                InternalName = tmp.Entity.InternalName,
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var template = await db.Templates.SingleOrDefaultAsync(t => t.Id == id);
            if (template == null)
            {
                return BadRequest(new
                { 
                    Message = $"Template with ID \"{id}\" not found"
                });
            }

            var configs = await db.TemplateConfigurations
                .Include(e => e.Lighthouse)
                .Where(e => e.TemplateId == id)
                .ToListAsync();
            if (configs.Count() > 0)
            {
                return BadRequest(new
                {
                    Message = "The following lighthouses depend on this template: " +
                        string.Join(", ", configs.Select(config => $"{config.Lighthouse.InternalName} ({config.LighthouseId})")) +
                        ".  Please delete them manually before deleting this template"
                });
            }

            var path = config.TemplateScriptPath(template);

            db.Templates.Remove(template);
            await db.SaveChangesAsync();

            System.IO.File.Delete(path);

            return Ok();
        }
    }
}
