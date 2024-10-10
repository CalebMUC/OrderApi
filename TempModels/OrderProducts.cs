using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class OrderProducts
    {
        public int Id { get; set; }

        // Foreign key to Transactions
        [ForeignKey("Order")]
        public string OrderID { get; set; }
        public Order order { get; set; }

        [ForeignKey("Product")]
        public string ProductID { get; set; }
        public TProduct Product { get; set; }

        public int Quantity { get; set; }
    }
}
