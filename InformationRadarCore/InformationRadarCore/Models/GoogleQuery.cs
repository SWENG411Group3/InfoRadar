using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public class GoogleQuery
    {
        public int Id { get; set; }

        public ICollection<Lighthouse> Lighthouses { get; set; }

        [Required]
        public string Query { get; set; }
    }
}
