using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        [Required]
        [ForeignKey(nameof(Payments))]
        public int PaymentMethodID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string TrxReference { get; set; } // CheckoutRequestID from M-Pesa

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string PaymentReference { get; set; } // MpesaReceiptNumber

        [Required]
        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Phonenumber { get; set; }


        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Status { get; set; } = "Pending"; // e.g. Pending, Success, Failed

        // ✅ Add this property for linking to an Order
        public string? OrderID { get; set; }


        [ForeignKey(nameof(OrderID))]
        public Orders Order { get; set; }

        // Navigation properties
        public PaymentMethods Payments { get; set; }
    }

}
