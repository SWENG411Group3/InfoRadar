namespace InformationRadarCore.Models.Web
{
    public class PaginatorQuery
    {
        public static int DEFAULT_SIZE = 10, MIN_SIZE = 5, MAX_SIZE = 100;

        private int _size = DEFAULT_SIZE;
        public int PageSize {
            get => _size; 
            set => _size = Math.Clamp(value, MIN_SIZE, MAX_SIZE); 
        }

        public int? Cursor { get; set; }
    }
}
