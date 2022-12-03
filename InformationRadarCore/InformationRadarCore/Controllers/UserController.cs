using Duende.IdentityServer.Extensions;
using InformationRadarCore.Data;
using InformationRadarCore.Models.Web;
using InformationRadarCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IAuthService auth;
        private readonly ApplicationDbContext db;

        public UserController(IAuthService auth, ApplicationDbContext db)
        {
            this.auth = auth;
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> UserData()
        {
            var user = await auth.GetUser();
            if (user == null)
            {
                return Unauthorized(new
                { 
                    Message = "Error getting signed in user",
                });
            }

            return Ok(new
            {
                user.Id,
                user.Email,
                user.IsAdmin,
            });
        }

        [HttpGet("List")]
        public async Task<IActionResult> ListUsers([FromQuery] PaginatorQuery<string> query)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var users = await db.Users
                .OrderByDescending(e => e.Id)
                .Where(e =>
                    (string.IsNullOrEmpty(query.Cursor) || string.Compare(e.Id, query.Cursor) < 0) && e.EmailConfirmed
                )
                .Select(e => new
                {
                    e.Id,
                    e.Email,
                    e.IsAdmin,
                    Subscriptions = e.Lighthouses.Count(),
                })
                .Take(query.PageSize)
                .ToListAsync();

            string? cursor = users.LastOrDefault()?.Id;

            return Ok(new
            {
                Entries = users,
                IsComplete = !string.IsNullOrEmpty(cursor) && !await db.Users
                    .AnyAsync(e => string.Compare(e.Id, cursor) < 0 && e.EmailConfirmed),
                Cursor = cursor,
            });
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> SetPermissions(string id, [FromBody] AdminPayload body)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                { 
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var user = await db.Users.SingleOrDefaultAsync(user => user.Id == id);
            if (user == null)
            {
                return NotFound(new
                { 
                    Message = $"No such user with ID {id}",
                });
            }

            user.IsAdmin = body.IsAdmin;
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
