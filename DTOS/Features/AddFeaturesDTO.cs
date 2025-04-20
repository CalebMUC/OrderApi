namespace Minimart_Api.DTOS.Features
{
    public class AddFeaturesDTO
    {
        public int CategoryID { get; set; }
        public int SubCategoryID { get; set; }
        public List<FeatureDTO> Features { get; set; }
    }
}
