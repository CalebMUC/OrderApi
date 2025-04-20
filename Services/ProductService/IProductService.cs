using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<Products>> GetAllProducts();
        Task<Status> EditProductsAsync(AddProducts products);
        Task<IEnumerable<Products>> FetchAllProducts();
        Task<Status> AddProducts(AddProducts product);
        Task<IEnumerable<CartResults>> GetProductsByCategory(int CategoryID);
        Task<IEnumerable<Products>> LoadProductImages(string ProductID);
    }
}
