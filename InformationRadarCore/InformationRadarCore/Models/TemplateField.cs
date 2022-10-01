using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models
{
    public enum TemplateFieldType 
    { 
        AnyJson,
        Int,
        Float,
        Date,
        String,
        StringList,
    }

    [Index(nameof(Name), nameof(TemplateId), IsUnique = true)]
    public class TemplateField
    {
        public int Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z]\w*$")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public int TemplateId { get; set; }
        public Template Template { get; set; }

        [Required]
        public TemplateFieldType DataType { get; set; }
    }
}
