using Minimart_Api.DTOS.Category;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services.CategoriesService
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Categories>> GetAllCategoriesAsync();
        Task<IEnumerable<Categories>> GetNestedCategoriesAsync();
        Task<Categories> GetCategoryByIdAsync(int CategoryId);
        Task<ResponseStatus> AddCategoriesAsync(CategoriesDto categories);
        Task<ResponseStatus> UpdateCategoriesAsync(CategoriesDto categories);
        Task<ResponseStatus> DeleteCategoryAsync(int CategoryId);
    }
}
