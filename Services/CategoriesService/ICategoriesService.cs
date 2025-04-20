using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Category;
using Minimart_Api.DTOS.General;
using Minimart_Api.Models;

namespace Minimart_Api.Services.CategoriesService
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Categories>> GetAllCategoriesAsync();
        Task<IEnumerable<Categories>> GetNestedCategoriesAsync();
        Task<Categories> GetCategoryByIdAsync(int CategoryId);
        Task<Status> AddCategoriesAsync(CategoriesDto categories);
        Task<Status> UpdateCategoriesAsync(CategoriesDto categories);
        Task<Status> DeleteCategoryAsync(int CategoryId);
        Task<IEnumerable<CartResults>> GetSubCategory(int CategoryId);
    }
}
