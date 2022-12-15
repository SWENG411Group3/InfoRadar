namespace InformationRadarCore.Models.Web
{
    public class PatchLighthousePayload : TaggedPayload
    {
        public bool? Enabled { get; set; }
        public ulong? Frequency { get; set; }
        public ulong? MessengerFrequency { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public void MapLighthouse(Lighthouse lighthouse)
        {
            if (Enabled.HasValue)
            {
                lighthouse.Enabled = Enabled.Value;
            }

            if (Title != null)
            {
                lighthouse.Title = Title;
            }

            if (Description != null)
            {
                lighthouse.Description = Description;
            }

            if (Frequency.HasValue)
            {
                lighthouse.Frequency = Frequency.Value;
            }

            if (MessengerFrequency.HasValue)
            {
                lighthouse.MessengerFrequency = MessengerFrequency.Value;
            }
        }
    }
}
