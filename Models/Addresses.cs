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
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string Phonenumber { get; set; }

        [Column(TypeName = "nvarchar(200)")]
        public string PostalAddress { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string County { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Town { get; set; }

        [Column(TypeName = "varchar(10)")]
        public string PostalCode { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string ExtraInformation { get; set; }

        [Required]
        //[Column(TypeName = "int")]
        public bool isDefault { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }

        // Navigation property
        [ForeignKey("UserID")]
        public Users users { get; set; }

    }
}
