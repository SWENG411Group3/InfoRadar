using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public class TemplateDefaultSite
    {
        public int Id { get; set; }

        [Required]
        public Template Template { get; set; }
        public int TemplateId { get; set; }

        [Required, MaxLength(2048)]
        public string Url { get; set; }
    }
}
