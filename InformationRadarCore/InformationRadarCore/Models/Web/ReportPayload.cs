using Duende.IdentityServer.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationRadarCore.Models.Web
{
    public class ReportPayload
    {
        [Range(1, 1_000_000)]
        public int PageSize { get; set; } = 500_000;
        [Range(1, int.MaxValue)]
        public int? Pages  { get; set; }
        [RegularExpression(@"^(csv|json|JSON|CSV|Json|Csv)$")]
        public string Type { get; set; } = "json";

        [NotMapped]
        public bool IsJson {
            get => string.Equals(Type, "json", StringComparison.InvariantCultureIgnoreCase);
        }

        [NotMapped]
        public ReportType ReportType { get => IsJson ? ReportType.Json : ReportType.Csv; }
    }
}
