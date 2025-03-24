using Minimart_Api.DTOS.Category;
using Minimart_Api.Repositories.CategoriesRepository;
using Minimart_Api.TempModels;

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
        public async Task<ResponseStatus> AddCategoriesAsync(CategoriesDto categories) { 
            return await _categoryRepos.AddCategoriesAsync(categories);
        }
        public async Task<ResponseStatus> UpdateCategoriesAsync(CategoriesDto categories)
        {
            return await _categoryRepos.UpdateCategoriesAsync(categories);
        }
        public async Task<ResponseStatus> DeleteCategoryAsync(int categoryId) { 
            return await _categoryRepos.DeleteCategoryAsync(categoryId);
        }
    }
}
