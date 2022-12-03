namespace InformationRadarCore.Models.Web
{
    public class CursorResponse<T, C>
    {
        public IList<T> Entries { get; set; }
        public bool IsComplete { get; set; }
        public C? Cursor { get; set; }
    }
}
