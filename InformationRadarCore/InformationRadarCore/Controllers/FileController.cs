using InformationRadarCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace InformationRadarCore.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class FileController : Controller
    {
        private static string[] supportedThumbnailTypes = { ".png", ".jpg" };
        public static string INVALID_IMG_MSG = "Invalid thumbnail.  Thumbnail must be jpg or png and less than 500KB";

        private readonly IAuthService auth;
        private readonly IWebHostEnvironment env;
        private readonly ConfigService config;

        public FileController(IAuthService auth, IWebHostEnvironment env, ConfigService config)
        {
            this.auth = auth;
            this.env = env;
            this.config = config;
        }

        [HttpPost("Thumbnail")]
        public async Task<IActionResult> UploadThumbnail(IFormFile file)
        {
            if (!await auth.IsAdmin())
            {
                return Unauthorized(new
                {
                    Message = AuthService.UNAUTHORIZED,
                });
            }

            var ext = Path.GetExtension(file.FileName);
            if (!supportedThumbnailTypes.Contains(ext) || file.Length > 500000)
            {
                return BadRequest(new
                { 
                    Message = INVALID_IMG_MSG,
                });
            }

            var filename = Path.GetRandomFileName() + ext;
            var path = Path.Combine(env.WebRootPath, config.ImageDir, filename);

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new 
            { 
                Filename = filename,
            });
        }
    }
}
