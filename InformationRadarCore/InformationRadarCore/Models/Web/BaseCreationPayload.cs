
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

        public bool ValidateImg(IWebHostEnvironment env, ConfigService config)
        {
            return string.IsNullOrEmpty(Thumbnail) || File.Exists(Path.Combine(env.WebRootPath, config.ImageDir, Thumbnail));
        }
    }
}
