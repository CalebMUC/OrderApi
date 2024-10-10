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

        public DateTime TrackingDate { get; set; } = DateTime.Now;
        public DateTime ExpectedDeliveryDate { get; set; }
        //foreign keys from orderStatus Tracking
        [ForeignKey("OrderStatusTracking")]
        public int PreviousStatus { get; set; }
        //foreign keys from orderStatus Tracking
        [ForeignKey("OrderStatusTracking")]
        public int CurrentStatus { get; set; }

        public string Carrier { get; set; }

        public ICollection<OrderStatusTracking> orderStatusTrackings { get; set; }
    }
}
