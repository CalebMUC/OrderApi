using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class SystemMerchants
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MerchantID { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string BusinessType { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string BusinessRegistrationNo { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]  // Changed from nvarchar
        public string KRAPIN { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string BusinessNature { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string BusinessCategory { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string MerchantName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        [Column(TypeName = "varchar(15)")]  // Changed from nvarchar
        public string Phone { get; set; }

        [Required]
        [StringLength(500)]
        [Column(TypeName = "varchar(500)")]  // Changed from nvarchar
        public string Address { get; set; }

        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string SocialMedia { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
        public string BankName { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]  // Changed from nvarchar
        public string BankAccountNo { get; set; }

        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
        public string BankAccountName { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]  // Changed from nvarchar
        public string MpesaPaybill { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]  // Changed from nvarchar
        public string MpesaTillNumber { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string PreferredPaymentChannel { get; set; }

        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string KRAPINCertificate { get; set; }

        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string BusinessRegistrationCertificate { get; set; }

        public bool TermsAndCondition { get; set; }

        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]  // Changed from nvarchar
        public string DeliveryMethod { get; set; }

        public bool ReturnPolicy { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]  // Changed from nvarchar
        public string Status { get; set; } = "Active";
    }
}