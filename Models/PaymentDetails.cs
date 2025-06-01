using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; } // Unique ID for the payment detail

        [Required]
        [ForeignKey("Payments")]
        public int PaymentMethodID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string TrxReference { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string PaymentReference { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        public long Phonenumber { get; set; }

        [Required]
        [Column(TypeName = "money")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime PaymentDate { get; set; }

        // Navigation property for related payments
        public PaymentMethods Payments { get; set; }

        // Navigation property for related orders
        public ICollection<Orders> Orders { get; set; }
    }
}
