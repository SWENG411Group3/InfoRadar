using Microsoft.EntityFrameworkCore;
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
        private String lighthouseScript;
        private string internalName;
        private String writeError;
        private String errorFile;
        private bool enabled;
        private LighthouseSize baseSize;

        /// <summary>
        /// Default constructor that requires the Name be supplied and unique, 
        /// sets the estimated base size of the Lighthouse,
        /// and whether or not the Lighthouse is enabled or not.
        /// </summary>
        public Lighthouse (String name, bool enabled, LighthouseSize size)
        {
            this.InternalName = name;
            this.BaseSize = size;
            this.Enabled = enabled;
        }

        public int Id { get; set; }

        /// <summary>
        /// The lighthouse's internal name
        /// The associated DB table will be named [INTERNAL NAME]_Lighthouse
        /// The associated python script will be named [INTERNAL NAME]_lighthouse.py
        /// </summary>
        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z]*$")]
        public string InternalName 
        {
            get { return internalName; }
            set { this.internalName = value; } 
        }

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
        public LighthouseSize BaseSize 
        {
            get { return this.baseSize; }
            set { this.baseSize = value; } 
        }

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
        /// Last time lighthouse settings were changed
        /// </summary>
        public DateTime? LastUpdated { get; set; }

        /// <summary>
        /// Last time lighthouse sent a message out
        /// </summary>
        public DateTime? LastSentMessage { get; set; }

        /// <summary>
        /// Sets the boolean value of whether or not the lighthouse should run
        /// </summary>
        [Required]
        public bool Enabled 
        {
            get { return this.enabled; }
            set { this.enabled = value; } 
        }

        /// <summary>
        /// App users who will recieve a message
        /// The set methods are supplied by the ICollection<t> methods and the get methods are supplied by the IEnumerator<T> methods
        /// </summary>
        public ICollection<ApplicationUser> Recipients { set; }

        public IEnumerator<ApplicationUser> Recipients { get; }

        public ICollection<GoogleQuery> GoogleQueries { set; }

        public IEnumerator<GoogleQuery> GoogleQueries { get; }

        public ICollection<Site> Sites { set; }

        public IEnumerator<Site> Sites { get; }

        /// <summary>
        /// Gets and Sets the location properties of the Lighthouse python script attribute.
        /// e.g. "C:\Temp\[INTERNAL NAME]_lighthouse.py"
        /// </summary>
        public String LighthouseScript 
        {
            get {return this.lighthouseScript; }
            set {this.LighthouseScript = value; } 
        }

        /// <summary>
        /// Sets the writeError attribute and writes it to the error log file
        /// </summary>
        public WriteError
        {
            set 
            {
                this.writeError = value;
                this.WriteErrorOut ();
            }
        }

        /// <summary>
        /// Writes writeError attribute to the error log file
        /// </summary>
        private async Task WriteErrorOut()
        {
            //Open Error log and write file here
            using StreamWriter file = new( DateTime + "\t" + this.errorFile, append: true);
            await file.WriteLineAsync ( this.writeError );
            if (file.Dispose)
            {
                this.writeError = null;
            }
        }
    }
}
