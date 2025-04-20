using Minimart_Api.DTOS.Features;
using Minimart_Api.DTOS.General;

namespace Minimart_Api.Repositories.Features
{
    public interface IFeatureRepo
    {
        Task<Status> AddFeatures(AddFeaturesDTO addFeatures);

        Task<List<FeatureDTO>> GetFeatures(FeatureRequestDTO feature);

    }
}
