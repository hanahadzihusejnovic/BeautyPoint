namespace BeautyPoint.SearchObjects
{
    public class ServicePackageSearchObject : BaseSearchObject 
    {
        public string ServicePackageName { get; set; }
        public decimal? MinServicePackagePrice { get; set; }
        public decimal? MaxServicePackagePrice { get; set; }
    }
}
