using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MinimartDBContext _dbContext;
        public ProductRepository(MinimartDBContext dBContext) { 
            _dbContext = dBContext;

        }
        public async Task<IEnumerable<TProduct>> GetAllProducts() {
            return await _dbContext.TProducts.ToListAsync();
           
        }
    }
}
