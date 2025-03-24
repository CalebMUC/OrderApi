using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<TProduct>> GetAllProducts();

        Task<ResponseStatus> EditProductsAsync(AddProducts products);
    }


}
