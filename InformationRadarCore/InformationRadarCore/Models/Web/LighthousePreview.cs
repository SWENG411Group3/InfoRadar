namespace InformationRadarCore.Models.Web
{
    public class LighthousePreview
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string InternalName { get; set; }
        public bool Enabled { get; set; }
        public bool HasError { get; set; }
        public bool Running { get; set; }
        public bool Subscribed { get; set; }
        public string? Thumbnail { get; set; }
    }
}
