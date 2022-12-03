using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models.Web
{
    public class TemplatePayload
    {
        public class FieldData
        { 
            public string Type { get; set; }
            public string Description { get; set; }
        }

        [Required, MaxLength(300)]
        public string Title { get; set; }

        [Required, MaxLength(700)]
        public string Description { get; set; }

        [Required, RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Template name can only contain letters")]
        public string InternalName { get; set; }

        public IDictionary<string, string> Columns { get; set; }

        public IDictionary<string, FieldData> Fields { get; set; }

        public string Code { get; set; }
    }
}
