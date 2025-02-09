using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories
{
    public interface ICategoryRepo
    {
        //Task<ResponseStatus> AddFeatures(int SubCategoryID, List<FeatureDTO> features);

        Task<ResponseStatus> AddFeatures(AddFeaturesDTO addFeatures);

        Task<ResponseStatus> AddCategories(AddCategoryDTO categories);
        Task<List<FeatureDTO>> GetFeatures(FeatureRequestDTO feature);

        Task<IEnumerable<CartResults>> GetSearchProducts(string subCategoryID);

        Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts);
    }
}
