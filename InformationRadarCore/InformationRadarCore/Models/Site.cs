using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public enum SiteContentType
    {
        Html,
        Json,
        Rss,
    }

    public class Site
    {
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        /// <summary>
        /// The content type that should be expected from the URL
        /// </summary>
        [Required]
        public SiteContentType Content { get; set; }
    }
}
