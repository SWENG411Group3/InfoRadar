using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public enum ReportType
    { 
        Json,
        Csv,
    }

    public class GeneratedReport
    {
        public int Id { get; set; }
        [Required]
        public DateTime Created { get; set; }
        [Required]
        public ReportType ReportType { get; set; }
        [Required]
        public Lighthouse Lighthouse { get; set; }
        public int LighthouseId { get; set; }
        [Required, MaxLength(255)]
        public string FileName { get; set; }
    }
}
