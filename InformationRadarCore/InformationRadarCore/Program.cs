using InformationRadarCore.Data;
using InformationRadarCore.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

namespace InformationRadarCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var settings = builder.Configuration.GetSection("AppSettings");

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            var validIssuer = settings.GetValue<string>("ReactValidIssuer");  
            if (!string.IsNullOrEmpty(validIssuer))
            {
                builder.Services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options =>
                {
                    var config = new[] { validIssuer };
                    var validIssuers = options.TokenValidationParameters.ValidIssuers;

                    if (validIssuers == null)
                    {
                        validIssuers = config;
                    }
                    else
                    {
                        validIssuers = validIssuers.Concat(config);
                    }

                    options.TokenValidationParameters.ValidIssuers = validIssuers;
                });

            }
            

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}