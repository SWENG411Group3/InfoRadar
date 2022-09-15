using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public class GoogleQuery
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string EngineId { get; set; }

        [Required]
        public string Query { get; set; }
    }
}
