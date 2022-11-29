using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    [Index(nameof(TagName), IsUnique = true)]
    public class Tag
    {
        public int TagId { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public string TagName { get; set; }

        public ICollection<Lighthouse> Lighthouses { get; set; }
    }
}
