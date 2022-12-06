using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationRadarCore.Models
{
    [Index(nameof(InternalName), IsUnique = true)]
    public class Lighthouse
    {
        public int Id { get; set; }

        /// <summary>
        /// The lighthouse's internal name
        /// The associated DB table will be named [INTERNAL NAME]_Lighthouse
        /// The associated python script will be named [INTERNAL NAME]_lighthouse.py
        /// </summary>
        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]\w*$")]
        public string InternalName { get; set; }

        /// <summary>
        /// The human-readable title of the lighthouse
        /// </summary>
        [Required, MaxLength(300)]
        public string Title { get; set; }

        /// <summary>
        /// Lighthouse description
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Last time the scraper ran
        /// </summary>
        public DateTime? LastVisitorRun { get; set; }

        /// <summary>
        /// How often the lighthouse gathers data (measured in seconds)
        /// If this is left unspecified, the frequency will be automatically determined 
        /// based on the lighthouse's base size
        /// </summary>
        [Required]
        public ulong Frequency { get; set; }

        /// <summary>
        /// How often the lighthouse checks if a message should be sent to recipients
        /// this value is nullable and measured in seconds
        /// It will be the same as the frequency if let unspecified
        /// </summary>
        public ulong? MessengerFrequency { get; set; }

        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Last time lighthouse settings were changed
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Last time lighthouse sent a message out
        /// </summary>
        public DateTime? LastSentMessage { get; set; }

        /// <summary>
        /// Index of the latest log file
        /// </summary>
        public int LatestLog { get; set; } = 0;

        /// <summary>
        /// Whether or not the lighthouse should run
        /// </summary>
        [Required]
        public bool Enabled { get; set; }

        /// <summary>
        /// Bool set if an irrecoverable error has been thrown on lighthouse
        /// </summary>
        [Required]
        public bool HasError { get; set; } = false;

        [MaxLength(16)]
        public string? Thumbnail { get; set; }

        [Required]
        public bool Running { get; set; } = false;

        [Required]
        public bool SearchRunning { get; set; } = false;

        /// <summary>
        /// App users who will recieve a message
        /// </summary>
        public ICollection<ApplicationUser> Recipients { get; set; }

        public ICollection<GoogleQuery> GoogleQueries { get; set; }

        public ICollection<Site> Sites { get; set; }
        public ICollection<Tag> Tags { get; set; } 

        public TemplateConfiguration? TemplateConfig { get; set; }
    }
}
