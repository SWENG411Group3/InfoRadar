using InformationRadarCore.Services;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models.Web
{
    public class BaseCreationPayload : TaggedPayload
    {
        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Internal name can only contain letters")]
        public string InternalName { get; set; }
        [Required, MinLength(1), MaxLength(300)]
        public string Title { get; set; }
        public bool? Enabled { get; set; }
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public ulong Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        [MaxLength(16)]
        public string? Thumbnail { get; set; }
        public IList<string>? Sites { get; set; }

        public IEnumerable<Uri> ValidSites()
        {
            return Sites?.Select(e => new Uri(e)) ?? Enumerable.Empty<Uri>();
        }

        public bool ValidateImg(IWebHostEnvironment env, ConfigService config)
        {
            return string.IsNullOrEmpty(Thumbnail) || File.Exists(Path.Combine(env.WebRootPath, config.ImageDir, Thumbnail));
        }

        public void GenDirectories(IWebHostEnvironment env, ConfigService config)
        {
            Directory.CreateDirectory(Path.Combine(config.ResourceRoot, "Scraper", "logs", InternalName));
            Directory.CreateDirectory(Path.Combine(env.WebRootPath, "Reports", InternalName));
        }

        public void GenInitLog(ConfigService config)
        {
            File.Create(Path.Combine(config.ResourceRoot, "Scraper", "logs", InternalName, "log_0.txt")).Dispose();
        }
    }
}
