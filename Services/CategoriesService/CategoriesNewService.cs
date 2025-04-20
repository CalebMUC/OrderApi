using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Category;
using Minimart_Api.Repositories.CategoriesRepository;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;

namespace Minimart_Api.Services.CategoriesService
{
    public class CategoriesNewService : ICategoriesService
    {
        private readonly ICategoryRepos _categoryRepos;
        public CategoriesNewService(ICategoryRepos categoryRepos) {
            _categoryRepos = categoryRepos;
        }
        public async Task<IEnumerable<Categories>> GetAllCategoriesAsync() { 
            return await _categoryRepos.GetAllCategoriesAsync();
        }

        public async Task<IEnumerable<Categories>> GetNestedCategoriesAsync()
        {
            return await _categoryRepos.GetNestedCategoriesAsync();
        }
        public async Task<Categories> GetCategoryByIdAsync(int CategorId) { 

            return await _categoryRepos.GetCategoryByIdAsync(CategorId);
        }
        public async Task<Status> AddCategoriesAsync(CategoriesDto categories) { 
            return await _categoryRepos.AddCategoriesAsync(categories);
        }
        public async Task<Status> UpdateCategoriesAsync(CategoriesDto categories)
        {
            return await _categoryRepos.UpdateCategoriesAsync(categories);
        }
        public async Task<IEnumerable<CartResults>> GetSubCategory(int CategoryId)
        {
            return await _categoryRepos.GetSubCategory(CategoryId);
        }
        public async Task<Status> DeleteCategoryAsync(int categoryId) { 
            return await _categoryRepos.DeleteCategoryAsync(categoryId);
        }
    }
}
