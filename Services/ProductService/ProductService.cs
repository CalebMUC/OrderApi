using Microsoft.EntityFrameworkCore;
using Minimart_Api.Repositories.ProductRepository;

namespace Minimart_Api.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository) { 
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<TProduct>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();

        }
    }
}
