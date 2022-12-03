using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace InformationRadarCore.Models.Web
{
    public class TemplateLighthousePayload : BaseCreationPayload
    {
        [Required]
        public int TemplateId { get; set; }
        [Required]
        public JsonElement Params { get; set; }

        public string? FindInvalidParam(IList<TemplateField> fields)
        {
            foreach (var field in fields)
            {
                JsonElement prop;
                if (!Params.TryGetProperty(field.Name, out prop))
                {
                    return field.Name;
                }

                bool okay = true;
                switch (field.DataType)
                {
                    case TemplateFieldType.StringList:
                        okay = prop.ValueKind == JsonValueKind.Array && prop.EnumerateArray().All(e => e.ValueKind == JsonValueKind.String);
                        break;
                    case TemplateFieldType.String:
                        okay = prop.ValueKind == JsonValueKind.String;
                        break;
                    case TemplateFieldType.Float:
                        okay = prop.ValueKind == JsonValueKind.Number;
                        break;
                    case TemplateFieldType.Int:
                        {
                            int o;
                            okay = prop.TryGetInt32(out o);
                        }
                        break;
                    case TemplateFieldType.Date:
                        {
                            DateTime o;
                            okay = prop.TryGetDateTime(out o);
                        }
                        break;
                }

                if (!okay) {
                    return field.Name;
                }
            }

            return null;
        }
    }
}
