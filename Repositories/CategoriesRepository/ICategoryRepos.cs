using Minimart_Api.DTOS.Category;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.CategoriesRepository
{
    public interface ICategoryRepos
    {
        Task<IEnumerable<Categories>> GetAllCategoriesAsync();
        Task<Categories> GetCategoryByIdAsync(int CategoryId);
        Task<IEnumerable<Categories>> GetNestedCategoriesAsync();
        Task<ResponseStatus> AddCategoriesAsync(CategoriesDto categories);
        Task<ResponseStatus> UpdateCategoriesAsync(CategoriesDto categories);
        Task<ResponseStatus> DeleteCategoryAsync(int CategoryId);

    }
}
