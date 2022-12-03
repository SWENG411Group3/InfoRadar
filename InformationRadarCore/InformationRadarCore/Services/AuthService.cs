using Duende.IdentityServer.Extensions;
using InformationRadarCore.Data;
using InformationRadarCore.Models;
using Microsoft.EntityFrameworkCore;

namespace InformationRadarCore.Services
{
    public class AuthService : IAuthService
    {
        public static string UNAUTHORIZED = "Unauthorized";

        private readonly ApplicationDbContext dbCtx;
        private readonly IHttpContextAccessor httpCtx;

        public AuthService(ApplicationDbContext db, IHttpContextAccessor http)
        {
            dbCtx = db;
            httpCtx = http;
        }

        public async Task<ApplicationUser?> GetUser()
        {
            var http = httpCtx.HttpContext;
            if (http == null || !http.User.IsAuthenticated())
            {
                return null;
            }

            return await dbCtx.Users
                .SingleOrDefaultAsync(usr => usr.Id == http.User.GetDisplayName());
        }

        public async Task<bool> IsAdmin()
        {
            return (await GetUser())?.IsAdmin ?? false;
        }
    }
}
