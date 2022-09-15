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

        [Required]
        public SiteContentType Content { get; set; }
    }
}
