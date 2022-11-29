using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    [Index(nameof(LighthouseId), IsUnique = true)]
    public class TemplateConfiguration
    {
        public int Id { get; set; }

        [Required]
        public Template Template { get; set; }
        public int TemplateId { get; set; }

        [Required]
        public string Payload { get; set; }

        [Required]
        public Lighthouse Lighthouse { get; set; }
        public int LighthouseId { get; set; }
    }
}
