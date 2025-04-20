using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Orders
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? OrderID { get; set; }

        //[Column(TypeName = "int")]
        //public int? MerchantId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int UserID { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime DeliveryScheduleDate { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? OrderedBy { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int Status { get; set; }

        [Required]
        [ForeignKey("PaymentDetails")]
        public int PaymentID { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string PaymentConfirmation { get; set; } = string.Empty;

        [Column(TypeName = "float")]
        public double TotalOrderAmount { get; set; }

        [Column(TypeName = "float")]
        public double TotalPaymentAmount { get; set; }

        [Column(TypeName = "float")]
        public double TotalDeliveryFees { get; set; }

        [Column(TypeName = "float")]
        public double TotalTax { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ShippingAddress { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string ProductsJson { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string PickupLocation { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string PaymentDetailsJson { get; set; }

        public virtual Users User { get; set; }

        public ICollection<OrderTracking> OrderTrackings { get; set; }

        public ICollection<OrderStatus> OrderStatuses { get; set; }


        public ICollection<OrderProducts> OrderProducts { get; set; }
    }
}
