using Minimart_Api.DTOS.Products;

namespace Minimart_Api.DTOS.Orders
{
    public class OrderEvent
    {
        public string OrderID { get; set; }
        public int UserID { get; set; }
        public List<ProductDto> products { get; set; }
        public string UserEmail { get; set; }
        public string MerchantEmail { get; set; }

        public string MerchantName { get; set; }
        public string UserPhoneNumber { get; set; }
        public string MerchantPhoneNumber { get; set; }
        public string addresses { get; set; }
        public DateTime OrderDate { get; set; }
        //public string DeliveryMode { get; set; }
        //public string PaymentMethod { get; set; }

        public double Amount { get; set; }

    }
}
