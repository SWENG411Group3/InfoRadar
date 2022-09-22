﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{

    public enum LighthouseSize { 
        Small,
        Medium,
        Large,
        ExtraLarge,
    }

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
        [RegularExpression(@"^[a-zA-Z][a-zA-Z]*$")]
        public string InternalName { get; set; }

        /// <summary>
        /// The human-readable title of the lighthouse
        /// </summary>
        [Required, MaxLength(100)]
        public string Title { get; set; }

        public DateTime? LastVisitorRun { get; set; }

        /// <summary>
        /// The estimated size of the lighthouse
        /// </summary>
        [Required]
        public LighthouseSize BaseSize { get; set; }

        /// <summary>
        /// How often the lighthouse gathers data (measured in seconds)
        /// If this is left unspecified, the frequency will be automatically determined 
        /// based on the lighthouse's base size
        /// </summary>
        [Required]
        public ulong? Frequency { get; set; }

        /// <summary>
        /// How often the lighthouse checks if a message should be sent to recipients
        /// this value is nullable and measured in seconds
        /// It will be the same as the frequency if let unspecified
        /// </summary>
        public ulong? MessengerFrequency { get; set; }

        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Last time lighthouse sent a message out
        /// </summary>
        public DateTime? LastSentMessage { get; set; }

        /// <summary>
        /// Whether or not the lighthouse should run
        /// </summary>
        [Required]
        public bool Enabled { get; set; }

        /// <summary>
        /// App users who will recieve a message
        /// </summary>
        public List<ApplicationUser> Recipients { get; set; }

        public List<GoogleQuery> GoogleQueries { get; set; }

        public List<Site> Sites { get; set; }
    }
}