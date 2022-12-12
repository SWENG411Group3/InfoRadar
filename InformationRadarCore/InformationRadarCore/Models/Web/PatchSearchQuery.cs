using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models.Web
{
    public class PatchSearchQuery
    {
        [Required, Range(5, 50)]
        public int NumResults { get; set; }
    }
}
