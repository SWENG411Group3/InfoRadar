using CsvHelper;
using Duende.IdentityServer.Extensions;
using InformationRadarCore.Data;
using InformationRadarCore.Models;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ReportingEngineController : Controller
    {
        private readonly IAuthService auth;
        private readonly ApplicationDbContext db;
        private readonly IDynDb dynDb;
        private readonly IWebHostEnvironment env;
        private readonly ConfigService config;

        public ReportingEngineController(ApplicationDbContext db, IAuthService auth, IDynDb dynDb, IWebHostEnvironment env, ConfigService config)
        {
            this.db = db;
            this.auth = auth;
            this.dynDb = dynDb;
            this.env = env;
            this.config = config;
        }

        [HttpPost("ErrorLogs/{id}")]
        public async Task<IActionResult> ErrorLogs(int id, [FromQuery] int? pages)
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
                    Message = $"No lighthouse with ID {id}",
                });
            }

            byte[] fileBody = { };
            var tempName = Path.GetTempFileName();

            try {
                var bottomLog = Math.Max(lighthouse.LatestLog - (pages ?? lighthouse.LatestLog), 0);

                using (var archive = ZipFile.Open(tempName, ZipArchiveMode.Update))
                {
                    for (int page = lighthouse.LatestLog; page >= bottomLog; page--)
                    {
                        var entry = archive.CreateEntry($"log_{page}.txt", CompressionLevel.Optimal);

                        using (var logStream = System.IO.File.OpenRead(config.LogfilePath(lighthouse.InternalName, page)))
                        using (var entryStream = entry.Open())
                        {
                            await logStream.CopyToAsync(entryStream);
                        }
                    }
                }

                fileBody = await System.IO.File.ReadAllBytesAsync(tempName);
            } 
            finally
            {
                System.IO.File.Delete(tempName);
            }

            return File(fileBody, "application/zip", "errors.zip");
        }

        [HttpGet("Records/{id}")]
        public async Task<IActionResult> ViewRecords(int id, [FromQuery] PaginatorQuery<int?> query)
        {
            var lighthouse = await db.Lighthouses.SingleOrDefaultAsync(l => l.Id == id);
            if (lighthouse == null)
            {
                return NotFound(new
                {
                    Message = $"No lighthouse with ID {id}",
                });
            }

            var records = await dynDb.LighthouseRecords(lighthouse, query.PageSize, query.Cursor);
            int? cursor = records.Rows.LastOrDefault()?.Id;

            return Ok(new CursorResponse<GenericRecordTable.JsonRecord, int?>()
            { 
                Entries = records.ToJson().ToList(),
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await dynDb.CountRecords(lighthouse, cursor) > 0),
            });
        }

        [HttpGet("Reports")]
        public async Task<IActionResult> ViewReports([FromQuery] SearchQueryPaginatorQuery query)
        {

            var reports = await db.Reports
                .OrderByDescending(e => e.Id)
                .Where(e => 
                    (!query.Cursor.HasValue || e.Id < query.Cursor.Value) &&
                    (!query.Lighthouse.HasValue || query.Lighthouse.Value == e.LighthouseId)
                )
                .Select(e => new
                {
                    e.Id, e.Created, e.FileName, 
                    WebPath = $"/{config.ReportDir}/{e.Lighthouse.InternalName}/{e.FileName}",
                    ReportType = e.ReportType.ToString(),
                })
                .Take(query.PageSize)
                .ToListAsync();

            int? cursor = reports.LastOrDefault()?.Id;
            return Ok(new
            {
                Entries = reports,
                Cursor = cursor,
                IsComplete = !(cursor.HasValue && await db.Reports.AnyAsync(e => e.Id < cursor.Value && 
                    (!query.Lighthouse.HasValue || e.LighthouseId == query.Lighthouse.Value))),
            });
        }

        [HttpPost("Generate/{id}")]
        public async Task<IActionResult> GenerateReport(int id, [FromBody] ReportPayload body)
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
                    Message = $"No lighthouse with ID {id}",
                });
            }

            var now = DateTime.UtcNow;
            var reportName = $"report__{now.ToString("MM-dd-yyyy__HH-mm-ss")}.zip";

            var inserted = await db.Reports.AddAsync(new GeneratedReport()
            { 
                Lighthouse = lighthouse,
                Created = now,
                ReportType = body.ReportType,
                FileName = reportName,
            });

            var path = Path.Combine(env.WebRootPath, config.ReportDir, lighthouse.InternalName, reportName);

            using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
            {
                bool complete = false, isJson = body.IsJson;
                int? cursor = null;
                for (int page = 0; !complete && (!body.Pages.HasValue || page < body.Pages.Value); page++)
                {
                    var records = await dynDb.LighthouseRecords(lighthouse, body.PageSize, cursor);

                    if (records.Rows.Any())
                    {
                        cursor = records.Rows.LastOrDefault()?.Id;

                        var entry = archive.CreateEntry($"page_{page}.{(isJson ? "json" : "csv")}", CompressionLevel.Optimal);
                        using (var stream = entry.Open())
                        {
                            if (isJson)
                            {
                                await System.Text.Json.JsonSerializer.SerializeAsync(stream, records.ToJson());
                            }
                            else
                            {
                                using (var writer = new StreamWriter(stream))
                                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                {
                                    await csv.WriteRecordsAsync(records.ToCsv());
                                }
                            }
                        }
                    }
                    complete = !(cursor.HasValue && await dynDb.CountRecords(lighthouse, cursor) > 0);
                }
            }

            await db.SaveChangesAsync();

            return Ok(new
            { 
                Id = inserted.Entity.Id,
                FileName = inserted.Entity.FileName,
            });
        }
    }
}
