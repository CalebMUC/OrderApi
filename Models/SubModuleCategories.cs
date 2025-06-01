using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class SubModuleCategories
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubCategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string SubCategoryName { get; set; }

        [Required]
        public int SubModuleID { get; set; }

        [MaxLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string SubCategoryUrl { get; set; }

        public int Order { get; set; }

        // Navigation property to SubModules
        [ForeignKey("SubModuleID")]
        public SubModules Submodule { get; set; }
    }
}
