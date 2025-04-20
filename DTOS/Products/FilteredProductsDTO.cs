namespace Minimart_Api.DTOS.Products
{
    public class FilteredProductsDTO
    {
        public int CategoryID { get; set; }
        public string SubCategoryID { get; set; }
        public Dictionary<string, string[]> features { get; set; }

    }
}
