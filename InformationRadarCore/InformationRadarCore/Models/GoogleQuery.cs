using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public class GoogleQuery
    {
        public int Id { get; set; }

        [Required, Range(5, 50)]
        public int NumResults { get; set; } = 10;

        [Required]
        public Lighthouse Lighthouse { get; set; }
        public int LighthouseId { get; set; }

        [Required]
        public string Query { get; set; }
    }
}