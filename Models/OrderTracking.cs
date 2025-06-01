using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class OrderTracking
    {
        [Key]
        [Column(TypeName = "varchar(50)")]
        public string TrackingID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string OrderID { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        public string ProductID { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime TrackingDate { get; set; } = DateTime.Now;

        [Column(TypeName = "timestamp")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Required]
        public int PreviousStatus { get; set; }

        [ForeignKey("PreviousStatus")]
        public OrderStatus PreviousStatusNavigation { get; set; }

        [Required]
        public int CurrentStatus { get; set; }

        [ForeignKey("CurrentStatus")]
        public OrderStatus CurrentStatusNavigation { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string Carrier { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime UpdatedOn { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products product { get; set; }
    }

}
