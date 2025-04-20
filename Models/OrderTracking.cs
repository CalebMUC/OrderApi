using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class OrderTracking
    {
        [Key]
        [Column(TypeName = "nvarchar(50)")]
        public string TrackingID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string OrderID { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string ProductID { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TrackingDate { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime ExpectedDeliveryDate { get; set; }

        [Required]
        public int PreviousStatus { get; set; }

        [ForeignKey("PreviousStatus")]
        public OrderStatus PreviousStatusNavigation { get; set; }

        [Required]
        public int CurrentStatus { get; set; }

        [ForeignKey("CurrentStatus")]
        public OrderStatus CurrentStatusNavigation { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Carrier { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products product { get; set; }
    }

}
