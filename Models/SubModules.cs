using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class SubModules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubModuleID { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string SubModuleName { get; set; }

        [Required]
        public int ModuleID { get; set; } // Foreign Key to Modules

        [MaxLength(200)]
        [Column(TypeName = "nvarchar(200)")]
        public string SubModuleUrl { get; set; }

        [Required]
        public int Order { get; set; } = 0;

        // Navigation Property for Module
        [ForeignKey("ModuleID")]
        public Modules Module { get; set; }

        // Navigation Property for RolePermissions
        public ICollection<RolePermissions> RolePermissions { get; set; } = new List<RolePermissions>();

        public ICollection<SubModuleCategories> SubModuleCategories { get; set; } = new List<SubModuleCategories>();
    }
}
