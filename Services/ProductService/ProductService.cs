using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Repositories.ProductRepository;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;

namespace Minimart_Api.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository) { 
            _productRepository = productRepository;
        }
        public async Task<IEnumerable<Products>> GetAllProducts()
        {
            return await _productRepository.GetAllProducts();

        }

        public async Task<Status> EditProductsAsync(AddProducts products)
        {
            return await _productRepository.EditProductsAsync(products);

        }
        public async Task<IEnumerable<Products>> FetchAllProducts()
        {
            return await _productRepository.FetchAllProducts();
        }

        public async Task<Status> AddProducts(AddProducts product)
        {
            return await _productRepository.AddProducts(product);
        }
        public async Task<IEnumerable<CartResults>> GetProductsByCategory(int CategoryID)
        {
            return await _productRepository.GetProductsByCategory(CategoryID);
        }
        public async Task<IEnumerable<Products>> LoadProductImages(string productID)
        {
            return await _productRepository.LoadProductImages(productID);
        }
    }
}
