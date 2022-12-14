using IdentityModel;
using InformationRadarCore.Data;
using InformationRadarCore.Models;
using InformationRadarCore.Services;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            builder.Services.AddAuthentication()
                .AddIdentityServerJwt();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDynDb, DynDb>();

            var configService = new ConfigService();
            settings.Bind(configService);
            builder.Services.AddSingleton(configService);

            builder.Services.AddSingleton<IScrapyInterface, ScrapyInterface>();

            var validIssuer = settings.GetValue<string>("ReactValidIssuer");  
            builder.Services.Configure<JwtBearerOptions>(IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
                options =>
                {
                    if (!string.IsNullOrEmpty(validIssuer))
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
                    }
                    options.TokenValidationParameters.AuthenticationType = "ApplicationCookie";
                    options.TokenValidationParameters.NameClaimType = ClaimTypes.NameIdentifier;
                    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                });

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

            app.UseIdentityServer();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapFallbackToFile("index.html");


            if (app.Services.GetRequiredService<IScrapyInterface>().CreateEnv() != 0)
            {
                throw new Exception("Could not set up scrapy env");
            }

            Directory.CreateDirectory(Path.Combine(configService.ResourceRoot, "Old", "Scripts"));
            Directory.CreateDirectory(Path.Combine(configService.ResourceRoot, "Scraper", "scripts", "templates"));
            Directory.CreateDirectory(Path.Combine(configService.ResourceRoot, "Scraper", "logs"));
            Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, configService.ImageDir));
            Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, configService.ReportDir));

            app.Run();
        }

    }
}