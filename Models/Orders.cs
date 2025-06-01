using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Orders
    {
        [Key]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string OrderID { get; set; }  // Removed nullable (primary key shouldn't be null)

        [Required]
        public int UserID { get; set; }  // Removed explicit type as int maps to integer

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
        public int Status { get; set; }  // Consider using enum instead of raw int

        [Required]
        [ForeignKey("PaymentDetails")]
        public int PaymentID { get; set; }

        public PaymentDetails PaymentDetails { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string PaymentConfirmation { get; set; } = string.Empty;

        [Column(TypeName = "money")]  // More precise than float
        public double TotalOrderAmount { get; set; }

        [Column(TypeName = "money")]
        public double TotalPaymentAmount { get; set; }

        [Column(TypeName = "money")]
        public double TotalDeliveryFees { get; set; }

        [Column(TypeName = "money")]
        public double TotalTax { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Column(TypeName = "money")]
        //public decimal NetAmount => TotalOrderAmount + TotalTax + TotalDeliveryFees;

        [Column(TypeName = "text")]  // Changed from nvarchar(max)
        public string? ShippingAddress { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]  // Better than text for JSON data
        public string ProductsJson { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string PickupLocation { get; set; }

        [Required]
        [Column(TypeName = "jsonb")]  // Better for JSON data
        public string PaymentDetailsJson { get; set; }

        // Navigation properties
        public virtual Users User { get; set; }
        public ICollection<OrderTracking> OrderTrackings { get; set; }
        public ICollection<OrderStatus> OrderStatuses { get; set; }
        public ICollection<OrderProducts> OrderProducts { get; set; }
    }
}