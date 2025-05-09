namespace BeautyPoint.SearchObjects
{
    public class TreatmentSearchObject : BaseSearchObject
    {
        public string? ServiceName { get; set; }
        public string? ServiceCategory { get; set; }
        public decimal? ServiceMinPrice { get; set; }
        public decimal? ServiceMaxPrice { get; set;}
    }
}
