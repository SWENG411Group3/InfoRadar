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
                options.UseSqlServer(connectionString), ServiceLifetime.Transient);
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
                options.SignIn.RequireConfirmedAccount = true;
            })
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();
            builder.Services.AddTransient<UserManager<ApplicationUser>>();

            builder.Services.AddAuthentication("Bearer")
                .AddIdentityServerJwt();

            builder.Services.AddLocalApiAuthentication();

            var configService = new ConfigService();
            settings.Bind(configService);
            builder.Services.AddSingleton(configService);

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

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.MapFallbackToFile("index.html");

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                roleSetup(roleManager);

                if (app.Environment.IsDevelopment() && !string.IsNullOrEmpty(configService.SeedAdmin))
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var admin = userManager.FindByEmailAsync(configService.SeedAdmin).Result;
                    userManager.AddToRoleAsync(admin, "Admin").Wait();
                }
                

            }

            app.Run();
        }

        public static void roleSetup(RoleManager<IdentityRole> manager)
        {
            // Populate DB with default roles
            if (!manager.RoleExistsAsync("Admin").Result)
            {
                manager.CreateAsync(new IdentityRole("Admin")).Wait();
            }
        }
    }
}