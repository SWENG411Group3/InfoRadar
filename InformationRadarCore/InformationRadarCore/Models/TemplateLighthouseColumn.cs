using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationRadarCore.Models
{

    [Index(nameof(Name), nameof(TemplateId), IsUnique = true)]
    public class TemplateLighthouseColumn
    {
        public int Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]\w*$")]
        public string Name { get; set; }

        [Required]
        public string DataType { get; set; }

        public int TemplateId { get; set; }
        public Template Template { get; set; }
    }
}
