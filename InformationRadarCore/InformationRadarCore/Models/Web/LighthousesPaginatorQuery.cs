namespace InformationRadarCore.Models.Web
{
    public class LighthouseSearchQuery
    {
        public string? Search { get; set; }
        public string? Tags { get; set; }
        public IList<string> FixedTags {
            get => (Tags ?? "").Split(',')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(x => x.Trim().ToLower())
                .ToList();
        }
    }
}
