namespace Minimart_Api.DTOS
{
    public class AddProducts
    {
        public string productName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string subcategory { get; set; } = string.Empty;

        public string productDetails { get; set; } = string.Empty;

        public string productSpecifications { get; set; } = string.Empty;

        
      public string productFeatures { get; set; } = string.Empty;

        public string boxContent { get; set; } = string.Empty;

        public decimal price { get; set; }

        public int Quantity { get; set; }

        public decimal discount { get; set; } 

        public string productImage { get; set; } = string.Empty;

        
    }
}
