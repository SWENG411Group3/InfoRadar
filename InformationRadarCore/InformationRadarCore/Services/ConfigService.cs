using InformationRadarCore.Models;
using System.IO;

namespace InformationRadarCore.Services
{
    public class ConfigService
    {
        public string? ReactValidIssuer { get; set; }
        public string ResourceRoot { get; set; }
        public string ImageDir { get; set; }
        public string? SeedAdmin { get; set; }

        public string CustomScriptPath(Lighthouse lighthouse)
        {
            return Path.Combine(ResourceRoot, "Scraper", "scripts", lighthouse.InternalName + "_lighthouse.py");
        }
        public string TemplateScriptPath(Template lighthouse)
        {
            return Path.Combine(ResourceRoot, "Scraper", "scripts", "templates", lighthouse.InternalName + "_template.py");
        }
    }
}
