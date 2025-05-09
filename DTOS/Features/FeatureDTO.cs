namespace Minimart_Api.DTOS.Features
{
    public class FeatureDTO
    {
        public string FeatureName { get; set; }
        public Dictionary<string, List<string>> FeatureOptions { get; set; }

        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? SubSubCategoryId { get; set; }
    }
}
