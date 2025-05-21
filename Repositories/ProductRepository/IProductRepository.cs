using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<Products>> GetAllProducts();

        Task<Status> EditProductsAsync(AddProducts products);

        Task<IEnumerable<CartResults>> GetProductsByCategory(int? CategoryID);


        Task<IEnumerable<Products>> FetchAllProducts();

        Task<Status> AddProducts(AddProducts product);

     

        Task<IEnumerable<Products>> LoadProductImages(string ProductID);

        Task<Products> GetByIdAsync(string productId);
        Task<IEnumerable<Products>> GetProductsByIdsAsync(IEnumerable<string> productIds);
        Task<IEnumerable<Products>> GetProductsByCategoryAsync(int categoryId, int limit, string excludeProductId);
        Task<IEnumerable<Products>> GetProductsBySubCategoryAsync(int subCategoryId, int limit, string excludeProductId);
        Task<IEnumerable<Products>> GetProductsByKeywordsAsync(IEnumerable<string> keywords, int limit, string excludeProductId);
        Task<IEnumerable<Products>> GetPopularProductsAsync(int limit, string excludeProductId);

    }


}
