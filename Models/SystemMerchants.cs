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
        [Column(TypeName = "nvarchar(255)")]
        public string BusinessName { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string BusinessType { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string BusinessRegistrationNo { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string KRAPIN { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string BusinessNature { get; set; }

        [Required]
        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string BusinessCategory { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string MerchantName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [StringLength(15)]
        [Column(TypeName = "nvarchar(15)")]
        public string Phone { get; set; }

        [Required]
        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string Address { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string SocialMedia { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string BankName { get; set; }

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string BankAccountNo { get; set; }

        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string BankAccountName { get; set; }

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string MpesaPaybill { get; set; }

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string MpesaTillNumber { get; set; }

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string PreferredPaymentChannel { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string KRAPINCertificate { get; set; }

        [StringLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string BusinessRegistrationCertificate { get; set; }

        public bool TermsAndCondition { get; set; } 

        [StringLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string DeliveryMethod { get; set; }

        public bool ReturnPolicy { get; set; }

        [StringLength(20)]
        [Column(TypeName = "nvarchar(20)")]
        public string Status { get; set; } = "Active";
    }
}
