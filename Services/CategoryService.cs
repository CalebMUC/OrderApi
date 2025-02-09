using Minimart_Api.DTOS;
using Minimart_Api.Repositories;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo ;
        public CategoryService(ICategoryRepo categoryRepo) {
            _categoryRepo = categoryRepo ;
        }

        //public async Task<ResponseStatus> AddFeatures(int SubCategoryID, List<FeatureDTO> features) {
        public async Task<ResponseStatus> AddFeatures(AddFeaturesDTO addFeatures)
        {
            return await _categoryRepo.AddFeatures(addFeatures);
        }
        public async Task<ResponseStatus> AddCategories( AddCategoryDTO categories)
        {

            return await _categoryRepo.AddCategories(categories);
        }
        public async Task<IEnumerable<CartResults>> GetSearchProducts(string subCategoryID)
        {
            return await _categoryRepo.GetSearchProducts(subCategoryID);
        }

        public async Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts)
        {
            return await _categoryRepo.GetFilteredProducts(filteredProducts);
        }

        public async Task<List<FeatureDTO>> GetFeatures(FeatureRequestDTO feature)
        {

             return await _categoryRepo.GetFeatures(feature);
        }
    }
}
