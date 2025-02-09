using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.TempModels
{
    public class Payments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        public string Name { get; set; }
        public string Description { get; set; } // Optional description of the payment method
        public bool IsActive { get; set; } = true; // To track if the payment method is active
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Creation date

        // Navigation property for related payment details
        public ICollection<PaymentDetails> PaymentDetails { get; set; }
    }

    public class PaymentDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentMethodID { get; set; } // Unique ID for the payment detail (not the method)

        [ForeignKey("Payments")]
        public int PaymentID { get; set; }

        public string TrxReference { get; set; }

        public string PaymentReference{ get; set; }

        public long Phonenumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }

        // Navigation property for related payments
        public Payments Payments { get; set; }

        // Navigation property for related orders
        public ICollection<Order> Orders { get; set; }
    }

}
