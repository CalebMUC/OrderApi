namespace Minimart_Api.DTOS.Merchants
{
    public class OrderTrackingDTO
    {
        public int StatusId { get; set; }
        public string Description { get; set; }
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string UpdatedBy { get; set; }

    }
}
