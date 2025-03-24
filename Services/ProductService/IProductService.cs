using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<TProduct>> GetAllProducts();

        Task<ResponseStatus> EditProductsAsync(AddProducts products);
    }
}
