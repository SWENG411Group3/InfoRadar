namespace InformationRadarCore.Models.Web
{
    public class CursorResponse<T>
    {
        public IList<T> Entries { get; set; }
        public bool IsComplete { get; set; }
        public int? Cursor { get; set; }
    }
}
