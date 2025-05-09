using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Features
    {
        [Key]
        public int FeatureID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "nvarchar(255)")]
        public string FeatureName { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string FeatureOptions { get; set; } // JSON stored as text

        [Required]
        [Column(TypeName = "int")]
        public int? CategoryID { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? SubCategoryID { get; set; }

        [Column(TypeName = "int")]
        public int? SubSubCategoryID { get; set; }

        // Optionally add navigation properties if needed:
        public virtual Categories Category { get; set; }
        public virtual Categories SubCategory { get; set; }
    }
}
