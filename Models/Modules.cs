using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Modules
    {
        [Key]
        public int ModuleID { get; set; } // Primary Key

        [Required]
        [MaxLength(150)]
        [Column(TypeName = "varchar(150)")]
        public string ModuleName { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string CreatedBy { get; set; }

        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string MenuUrl { get; set; }

        [Required]
        [Column(TypeName = "timestamp")]
        public DateTime CreatedOn { get; set; }

        // Navigation Properties
        public ICollection<SubModules> Submodules { get; set; } = new List<SubModules>();

        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();
    }
}
