using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface ICategoryService
    {
        Task<ResponseStatus> AddFeatures(AddFeaturesDTO addFeatures);

        Task<ResponseStatus> AddCategories(AddCategoryDTO categories);
        Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts);
        Task<List<FeatureDTO>> GetFeatures(FeatureRequestDTO feature);

        //Task<List<FeatureDTO>> FeatureSearchFilter(FeatureSe feature);

         Task<IEnumerable<Features>> GetAllFeatures();

        Task<IEnumerable<CartResults>> GetSearchProducts(string subCategoryID);

    }
}
