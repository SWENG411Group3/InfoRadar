
using InformationRadarCore.Services;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace InformationRadarCore.Models.Web
{
    public class BaseCreationPayload : TaggedPayload
    {
        private static string[] supportedThumbnailTypes = { ".png", ".jpg" };
        public static  string INVALID_IMG_MSG = "Invalid thumbnail.  Thumbnail must be jpg or png and less than 500KB";

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Internal name can only contain letters")]
        public string InternalName { get; set; }
        [Required, MinLength(1), MaxLength(300)]
        public string Title { get; set; }
        public bool? Enabled { get; set; }
        [Required]
        public ulong Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        public IFormFile? Thumbnail { get; set; }

        public bool ValidateImg()
        {
            return Thumbnail == null || 
                supportedThumbnailTypes.Contains(Path.GetExtension(Thumbnail.FileName)) && 
                Thumbnail.Length <= 500000;
        }

        public async Task UploadImage(string name, IWebHostEnvironment env, ConfigService config)
        {
            if (Thumbnail == null)
            {
                return;
            }

            var path = Path.Combine(env.WebRootPath, config.ImageDir, name);
            using (var stream = File.Create(path))
            {
                await Thumbnail.CopyToAsync(stream);
            }

        }
        
        public string? RandomFilename()
        {
            return Thumbnail == null ? null : Path.GetRandomFileName() + Path.GetExtension(Thumbnail.FileName);
        }
    }
}
