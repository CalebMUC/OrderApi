namespace Minimart_Api.DTOS
{
    public class FeatureDTO
    {
        public string FeatureName { get; set; }
        public Dictionary<string, List<string>> FeatureOptions{ get; set; }
    }
}
