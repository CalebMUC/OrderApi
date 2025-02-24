using Minimart_Api.DTOS;

namespace Minimart_Api.Repositories.ProductRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<TProduct>> GetAllProducts();
    }
}
