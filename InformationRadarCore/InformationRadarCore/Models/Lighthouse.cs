using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{

    [Index(nameof(InternalName), IsUnique = true)]
    public class Lighthouse
    {
        public int Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z]*$")]
        public string InternalName { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        public DateTime? LastVisitorRun { get; set; }

        public ulong? Frequency { get; set; }

        public ulong? MessengerFrequency { get; set; }

        [Required]
        public DateTime Created { get; set; }

        public DateTime? LastSentMessage { get; set; }

        public List<ApplicationUser> Recipients { get; set; }

        public List<GoogleQuery> GoogleQueries { get; set; }

        public List<Site> Sites { get; set; }
    }
}
