namespace Minimart_Api.DTOS
{
    public class AddProducts
    {
        public string productName { get; set; } = string.Empty;
        public string productID { get; set; } = string.Empty;
        public int CategoryID { get; set; }

        public string SearchKeyWord { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public string subcategory { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public string productDetails { get; set; } = string.Empty;

        public string productSpecifications { get; set; } = string.Empty;


        public string productFeatures { get; set; } = string.Empty;

        public string boxContent { get; set; } = string.Empty;

        public decimal price { get; set; }

        public int Quantity { get; set; }

        public double discount{ get; set; }

        public string productImage { get; set; } = string.Empty;
    }
}
