using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{

    public class Roles
    {
        [Key]
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string RoleID { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RoleName { get; set; }

        [Required]
        public int AccessLevel { get; set; }

        // Navigation Property for RolePermissions

        public ICollection<Users> Users { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}
