using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace InformationRadarCore.Models.Web
{
    public class SearchQueryPayload
    {
        [Required]
        public int Lighthouse { get; set; }

        [Required]
        public string Query { get; set; }

        [Range(5, 50)]
        public int NumResults { get; set; } = 10;

        [NotMapped]
        public string NormalizedQuery { 
            get {
                return Regex.Replace(Query.Trim().ToLower(), @"\s+", " ");
            } 
        }
    }
}
