namespace HoaHoeHoaSoi.API.ViewModels
{
    public class ProductVMs
    {
    }
    public class ProductFilterModel
    {
        public int Id { get; set; }
        public string Search { get; set; } = string.Empty; 
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool SortByBestSeller { get; set; } = false;
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double? Price { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
    }
}
