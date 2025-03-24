namespace Minimart_Api.DTOS
{
    public class CartResults
    {
        public string productID { get; set; }
        public int MerchantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;

        public string ProductDescription { get; set; } = string.Empty;
        public string KeyFeatures { get; set; } = string.Empty;
        public string Specification { get; set; } = string.Empty;
        public string Box { get; set; } = string.Empty;
        public int Quantity { get; set; }

        public decimal? price { get; set; } 

        public int Instock { get; set; }

        public int CartID { get; set; }

        public int CartItemID { get; set; }
    }
}
