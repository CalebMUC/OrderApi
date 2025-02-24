namespace Minimart_Api.Services.ProductService
{
    public interface IProductService
    {
        Task<IEnumerable<TProduct>> GetAllProducts();
    }
}
