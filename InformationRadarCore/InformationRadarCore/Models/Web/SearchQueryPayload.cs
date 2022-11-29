using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.RegularExpressions;

namespace InformationRadarCore.Models.Web
{
    public class SearchQueryPayload
    {
        public int Lighthouse { get; set; }
        public string Query { get; set; }

        [NotMapped]
        public string NormalizedQuery { 
            get {
                return Regex.Replace(Query.Trim().ToLower(), @"\s+", " ");
            } 
        }
    }
}
