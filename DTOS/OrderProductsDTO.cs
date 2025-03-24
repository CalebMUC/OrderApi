namespace Minimart_Api.DTOS
{
    public class OrderProductsDTO
    {
        public string ProductID { get; set; }
        public int merchantId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public double DeliveryFee { get; set; }

        public double Discount { get; set; }
        public string ImageUrl { get; set; }


    }
}
