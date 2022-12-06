using InformationRadarCore.Controllers;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models.Web
{
    public class CreateLighthousePayload : BaseCreationPayload
    {
        [Required]
        public IDictionary<string, string> Types { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
