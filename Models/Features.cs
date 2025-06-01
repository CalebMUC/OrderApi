using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Features
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeatureID { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string FeatureName { get; set; }

        [Column(TypeName = "jsonb")]  // Changed to jsonb for better JSON handling in PostgreSQL
        public string FeatureOptions { get; set; }

        [ForeignKey("Category")]
        public int? CategoryID { get; set; }

        [ForeignKey("SubCategory")]
        public int? SubCategoryID { get; set; }

        [ForeignKey("SubSubCategory")]
        public int? SubSubCategoryID { get; set; }

        // Navigation properties
        public virtual Categories Category { get; set; }
        public virtual Categories SubCategory { get; set; }
        public virtual Categories SubSubCategory { get; set; }
    }
}