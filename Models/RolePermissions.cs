using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class RolePermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RolePermissionID { get; set; } // Primary Key

        // Foreign Key to Roles
        [Required]
        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string RoleID { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string RoleName { get; set; }

        // Foreign Key to Modules
        [Required]
        public int ModuleID { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string ModuleName { get; set; }

        // Foreign Key to Submodules
        [Required]
        public int SubModuleID { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string SubModuleName { get; set; }

        // Navigation Properties
        [ForeignKey("RoleID")]
        public Roles Role { get; set; }

        [ForeignKey("ModuleID")]
        public Modules Module { get; set; }

        [ForeignKey("SubModuleID")]
        public SubModules Submodule { get; set; }
    }
}
