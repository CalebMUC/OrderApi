using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class SubCategoryFeatures
    {
        [Key]
        public int SubCategoryFeatureID { get; set; }

        public int SubCategoryId { get; set; }
        public int FeatureID { get; set; }

        // Correctly define the navigation properties with foreign keys
        [ForeignKey("SubCategoryId")]
        public virtual TSubcategoryid Subcategory { get; set; }

        [ForeignKey("FeatureID")]
        public virtual Features features { get; set; }
    }
}
