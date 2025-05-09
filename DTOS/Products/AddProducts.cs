namespace Minimart_Api.DTOS.Products
{
    public class AddProducts
    {
        public int merchantID { get; set; }
        public string productName { get; set; } = string.Empty;
        public string productID { get; set; } = string.Empty;
        public int categoryId { get; set; }  // Lowercase 'Id' to match JSON
        public string searchKeyWord { get; set; } = string.Empty;
        public string categoryName { get; set; } = string.Empty;
        public int subCategoryId { get; set; }  // Lowercase 'Id'
        public string subCategoryName { get; set; } = string.Empty;
        public int subSubCategoryId { get; set; }  // Lowercase 'Id'
        public string subSubCategoryName { get; set; } = string.Empty;
        public string createdBy { get; set; } = string.Empty;
        public string productDetails { get; set; } = string.Empty;
        public string[] productSpecifications { get; set; }  // Changed to array
        public Dictionary<string, string> productFeatures { get; set; }  // Changed to dictionary
        public string[] boxContent { get; set; }  // Changed to array
        public decimal price { get; set; }
        public int quantity { get; set; }  // Lowercase to match JSON
        public bool inStock { get; set; }  // Added missing field
        public decimal discount { get; set; }  // Changed to decimal
        public string[] imageUrls { get; set; }  // Renamed to match JSON
    }
}
