using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.Models
{
    public class Addresses
    {
        [Key]
        public int AddressID { get; set; }

        [Required]
        public int UserID { get; set; } // Foreign key

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        public string Phonenumber { get; set; }

        [MaxLength(200)]
        public string PostalAddress { get; set; }

        [MaxLength(50)]
        public string County { get; set; }

        [MaxLength(50)]
        public string Town { get; set; }

        [MaxLength(10)]
        public string PostalCode { get; set; }

        [MaxLength(500)]
        public string ExtraInformation { get; set; }

        [Required]
        public bool isDefault { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }

        // Navigation property
        [ForeignKey("UserID")]
        public Users users { get; set; }
    }
}
