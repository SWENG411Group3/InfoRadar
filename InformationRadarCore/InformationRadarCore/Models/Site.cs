using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public class Site
    {
        public int Id { get; set; }

        [Required]
        public Lighthouse Lighthouse { get; set; }
        public int LighthouseId { get; set; }

        [Required, MaxLength(2048)]
        public string Url { get; set; }

        /// <summary>
        /// The date this site was added
        /// </summary>
        [Required]
        public DateTime Created { get; set; }
    }
}
