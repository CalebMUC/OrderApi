using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Minimart_Api.Models.Enums;

namespace Minimart_Api.Models
{
    public class Orders
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime DeliveryScheduleDate { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? OrderedBy { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public OrderStatusEnum StatusEnum { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? StatusMessage { get; set; }

        [Required]
        [ForeignKey(nameof(PaymentDetails))]
        public int PaymentID { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string PaymentConfirmation { get; set; } = string.Empty;

        [Column(TypeName = "money")]
        public double TotalOrderAmount { get; set; }

        [Column(TypeName = "money")]
        public double TotalPaymentAmount { get; set; }

        [Column(TypeName = "money")]
        public double TotalDeliveryFees { get; set; }

        [Column(TypeName = "money")]
        public double TotalTax { get; set; }

        [Column(TypeName = "text")]
        public string? ShippingAddress { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public string ProductsJson { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string PickupLocation { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]
        public string PaymentDetailsJson { get; set; }

        // 🔹 Navigation properties
        public virtual Users User { get; set; }
        public ICollection<OrderTracking> OrderTrackings { get; set; }
        public ICollection<OrderStatus> OrderStatuses { get; set; }
        public ICollection<OrderProducts> OrderProducts { get; set; }
    }
}
