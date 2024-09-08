namespace Minimart_Api.DTOS
{
    public class AddToCart
    {
        public int Quantity { get; set; }
        public string? ProductID { get; set; }
        public string? UserID { get; set; }
    }
}
