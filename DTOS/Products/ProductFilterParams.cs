namespace Minimart_Api.DTOS.Products
{
    // ProductFilterParams.cs (new class)
    public class ProductFilterParams
    {
        public string SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Dictionary<string, string[]> Features { get; set; } = new();
    }
}
