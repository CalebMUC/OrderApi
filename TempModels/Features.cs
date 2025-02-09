using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.TempModels
{
    public class Features
    {
        [Key]
        public int  FeatureID { get; set; }
        public string FeatureName { get; set; }
        public string FeatureOptions { get; set; } //json

        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }

    }
}
