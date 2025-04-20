namespace Minimart_Api.DTOS.Merchants
{
    public class MerchantOrderDto
    {
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Status { get; set; }
        public string ProductID { get; set; }

    }
}
