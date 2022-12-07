using Duende.IdentityServer.EntityFramework.Options;
using InformationRadarCore.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace InformationRadarCore.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {}
        public DbSet<Lighthouse> Lighthouses { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<GoogleQuery> GoogleQueries { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<TemplateConfiguration> TemplateConfigurations { get; set; }
        public DbSet<TemplateField> TemplateFields { get; set; }
        public DbSet<TemplateLighthouseColumn> TemplateLighthouseColumns { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<GeneratedReport> Reports { get; set; }
    }
}