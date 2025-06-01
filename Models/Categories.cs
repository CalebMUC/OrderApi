using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(255)]
        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string CategoryName { get; set; }

        [Column(TypeName = "varchar(255)")]  // Changed from nvarchar
        public string Slug { get; set; }

        [Column(TypeName = "varchar(1000)")]  // Changed from nvarchar
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;  // Removed bit annotation (bool maps to boolean)

        public int? ParentCategoryId { get; set; }

        [Column(TypeName = "varchar(1000)")]  // Changed from nvarchar
        public string Path { get; set; }

        [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
        public string CreatedBy { get; set; }

        [Column(TypeName = "timestamp")]  // Changed from datetime
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Column(TypeName = "varchar(100)")]  // Changed from nvarchar
        public string UpdatedBy { get; set; }

        [Column(TypeName = "timestamp")]  // Changed from datetime
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        public Categories ParentCategory { get; set; }

        public ICollection<Categories> SubCategories { get; set; }

        public virtual ICollection<Features> CategoryFeatures { get; set; }
        public virtual ICollection<Features> SubCategoryFeatures { get; set; }
        public virtual ICollection<Features> SubSubCategoryFeatures { get; set; }

        public ICollection<Products> Products { get; set; }
    }
}