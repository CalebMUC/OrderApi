using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class OrderTracking
    {
        [Key]
        public string TrackingID { get; set; }
        //foreignkey from orders
        [ForeignKey("Orders")]
        public string OrderID { get; set; }
        [ForeignKey("Tproduct")]
        public string ProductID { get; set; }

        public DateTime TrackingDate { get; set; } = DateTime.Now;
        public DateTime ExpectedDeliveryDate { get; set; }
        //foreign keys from orderStatus Tracking
        [ForeignKey("OrderStatus")]
        public int PreviousStatus { get; set; }
        //foreign keys from orderStatus Tracking
        [ForeignKey("OrderStatus")]
        public int CurrentStatus { get; set; }

        public string Carrier { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public ICollection<OrderStatus> orderStatus { get; set; }
    }
}
