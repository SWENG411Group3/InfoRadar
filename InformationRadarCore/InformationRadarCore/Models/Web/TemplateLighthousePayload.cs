using Duende.IdentityServer.Models;
using InformationRadarCore.Data;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace InformationRadarCore.Models.Web
{
    public class TemplateLighthousePayload : BaseCreationPayload
    {
        [Required]
        public int TemplateId { get; set; }
        [Required]
        public JObject Params { get; set; }

        public string? FindInvalidParam(IList<TemplateField> fields)
        {
            foreach (var field in fields)
            {
                var prop = Params.GetValue(field.Name);
                if (prop == null)
                {
                    return field.Name;
                }

                bool okay = true;
                switch (field.DataType)
                {
                    case TemplateFieldType.StringList:
                        okay = prop.Type == JTokenType.Array && prop.Children().All(e => e.Type == JTokenType.String);
                        break;
                    case TemplateFieldType.String:
                        okay = prop.Type == JTokenType.String;
                        break;
                    case TemplateFieldType.Float:
                        okay = prop.Type == JTokenType.Float || prop.Type == JTokenType.Integer;
                        break;
                    case TemplateFieldType.Int:
                        okay = prop.Type == JTokenType.Integer;
                        break;
                    case TemplateFieldType.Date:
                        DateTime o;
                        okay = prop.Type == JTokenType.String && DateTime.TryParse(prop.Value<string>(), out o);
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
