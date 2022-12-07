using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    /// <summary>
    /// Temporary DB site list that gets passed between Spiders
    /// Results are fetched from search query results
    /// </summary>
    public class SearchResult
    {
        public int Id { get; set; }

        [Required]
        public Lighthouse Lighthouse { get; set; }
        public int LighthouseId { get; set; }

        [Required, MaxLength(2048)]
        public string Url { get; set; }
    }
}
