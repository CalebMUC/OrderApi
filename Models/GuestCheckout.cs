using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.Models
{
    /// <summary>
    /// Stores guest checkout delivery details before payment confirmation
    /// </summary>
    public class GuestCheckout
    {
        [Key]
        public Guid GuestCheckoutId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string GuestId { get; set; } = string.Empty;

        /// <summary>
        /// Guest's full name
        /// </summary>
        [Required]
        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Kenyan phone number (stored as 07XXXXXXXX or 01XXXXXXXX)
        /// </summary>
        [Required]
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        [RegularExpression(@"^0[17]\d{8}$", ErrorMessage = "Invalid Kenyan phone number. Must be 07XXXXXXXX or 01XXXXXXXX")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Email address (optional for guests)
        /// </summary>
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Full delivery address
        /// </summary>
        [Required]
        [Column(TypeName = "text")]
        public string DeliveryAddress { get; set; } = string.Empty;

        /// <summary>
        /// County ID for delivery location (INT not GUID)
        /// </summary>
        public int? CountyId { get; set; }

        /// <summary>
        /// Town ID for delivery location (INT not GUID)
        /// </summary>
        public int? TownId { get; set; }

        /// <summary>
        /// Delivery station/pickup location ID (INT not GUID)
        /// </summary>
        public int? DeliveryStationId { get; set; }

        /// <summary>
        /// Calculated delivery fee in KES
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal DeliveryFee { get; set; }

        /// <summary>
        /// Subtotal amount (products only)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal SubtotalAmount { get; set; }

        /// <summary>
        /// Total amount including delivery
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Status: Pending, Completed, Abandoned
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// M-Pesa CheckoutRequestID
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? CheckoutRequestId { get; set; }

        /// <summary>
        /// M-Pesa MerchantRequestID
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? MerchantRequestId { get; set; }

        /// <summary>
        /// M-Pesa transaction code (after successful payment)
        /// </summary>
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string? MpesaReceiptNumber { get; set; }

        /// <summary>
        /// Created Order ID after payment success
        /// </summary>
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string? OrderId { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? CompletedAt { get; set; }

        // Navigation properties
        [ForeignKey("CountyId")]
        public virtual Counties? County { get; set; }

        [ForeignKey("TownId")]
        public virtual Towns? Town { get; set; }

        [ForeignKey("DeliveryStationId")]
        public virtual DeliveryStations? DeliveryStation { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order? Order { get; set; }
    }
}
