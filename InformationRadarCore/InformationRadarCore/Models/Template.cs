using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{

    [Index(nameof(InternalName), IsUnique = true)]
    public class Template
    {
        public int Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z]*$")]
        public string InternalName { get; set; }

        public ICollection<TemplateField> TemplateFields { get; set; }
        public ICollection<TemplateLighthouseColumn> LighthouseColumns { get; set; }
        public ICollection<TemplateConfiguration> Configurations { get; set; }
    }
}
