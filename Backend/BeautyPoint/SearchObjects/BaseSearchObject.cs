namespace BeautyPoint.SearchObjects
{
    public class BaseSearchObject
    {
        private const int maxPageSize = 50;
    
        public int PageNumber { get; set; } = 1;
    
        private int _pageSize = 10;
    
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value < 1 ? 1 : (value > maxPageSize ? maxPageSize : value);
            }
        }
    }
}
