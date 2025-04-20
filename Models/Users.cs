using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string? UserName { get; set; }

        [MaxLength(15)]
        [Column(TypeName = "nvarchar(15)")]
        [Phone]
        public string? PhoneNumber { get; set; }

        public bool? IsLoggedIn { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? Password { get; set; }

        public bool? IsAdmin { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? PasswordChangesOn { get; set; }

        public int? FailedAttempts { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "nvarchar(50)")]
        public string? RoleId { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        [Column(TypeName = "nvarchar(100)")]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? Salt { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Navigation Properties
        public virtual Roles Role { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Reviews> Reviews { get; set; }
        public virtual ICollection<Addresses> Addresses { get; set; }
    }
}
