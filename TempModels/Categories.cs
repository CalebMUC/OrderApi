using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(255)]
        public string  CategoryName { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
        public int? ParentCategoryId { get; set; }
        [ForeignKey("ParentCategoryId")]
        public Categories ParentCategory { get; set; }//navigation property for categories
        public string Path { get; set; }
        public string  CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; } = DateTime.Now;

        public ICollection<Categories> SubCategories { get; set; }

    }
}
