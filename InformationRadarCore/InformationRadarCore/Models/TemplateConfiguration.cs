using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationRadarCore.Models
{
    public class TemplateConfiguration
    {
        public int Id { get; set; }

        [Required]
        public Template Template { get; set; }

        [Required]
        public string Payload { get; set; }

        public int LighthouseId { get; set; }
        public Lighthouse Lighthouse { get; set; }
    }
}
