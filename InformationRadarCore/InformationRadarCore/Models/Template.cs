using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{

    [Index(nameof(InternalName), IsUnique = true)]
    public class Template
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; }

        [MaxLength(700)]
        public string Description { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Template name can only contain letters")]
        public string InternalName { get; set; }

        public ICollection<TemplateField> TemplateFields { get; set; }
        public ICollection<TemplateLighthouseColumn> LighthouseColumns { get; set; }
        public ICollection<TemplateConfiguration> Configurations { get; set; }
    }
}
