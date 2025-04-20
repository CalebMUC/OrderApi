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
        [Column(TypeName = "nvarchar(255)")]
        public string CategoryName { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string Slug { get; set; }

        [Column(TypeName = "nvarchar(1000)")]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "bit")]
        public bool IsActive { get; set; } = true;

        public int? ParentCategoryId { get; set; }

        

        [Column(TypeName = "nvarchar(1000)")]
        public string Path { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [Column(TypeName = "nvarchar(100)")]
        public string UpdatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        public Categories ParentCategory { get; set; } // Navigation property

        //public ICollection<Categories> ChildCategories { get; set; }


        public ICollection<Categories> SubCategories { get; set; }

        public ICollection<Features> CategoryFeatures { get; set; }//collection of features in a category

        public ICollection<Features> SubCategoryFeatures { get; set; }//collection of features in a subcategory

        public ICollection<Products> Products { get; set; }
    }
}
