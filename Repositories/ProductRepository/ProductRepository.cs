using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MinimartDBContext _dbContext;
        public ProductRepository(MinimartDBContext dBContext)
        {
            _dbContext = dBContext;

        }
        public async Task<IEnumerable<TProduct>> GetAllProducts()
        {
            return await _dbContext.TProducts.ToListAsync();

        }

        public async Task<ResponseStatus> EditProductsAsync(AddProducts products)
        {
            try
            {
                var existingProduct = await _dbContext.TProducts.FindAsync(products.productID);
                updateProductFromDto(existingProduct, products);
                await _dbContext.SaveChangesAsync();

                return new ResponseStatus
                {
                    ResponseStatusId = 200,
                    ResponseMessage = "Products Edit Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseStatus
                {
                    ResponseStatusId = 500,
                    ResponseMessage = $"Products failed to  Edited {ex.Message} "
                };

            }
        }

        //update existing Entity from DTO
        public void updateProductFromDto(TProduct entity, AddProducts product)
        {
            // Update the product entity with values from the DTO
            entity.merchantID = product.merchantID;
            entity.ProductId = product.productID;
            entity.ProductName = product.productName;
            entity.ProductDescription = product.productDetails;
            entity.ProductType = "P"; // Assuming "P" is the default product type
            entity.CategoryId = product.CategoryID;
            entity.SubCategoryId = product.subcategory;
            entity.SearchKeyWord = product.SearchKeyWord;
            entity.Price = product.price;
            entity.InStock = product.Quantity;
            entity.Discount = product.discount;
            entity.ImageUrl = product.productImage;
            entity.KeyFeatures = product.productFeatures;
            entity.Box = product.boxContent;
            entity.Specification = product.productSpecifications;
            entity.CreatedOn = DateTime.Now; // Update the creation timestamp
            entity.CreatedBy = product.CreatedBy;
            entity.ImageType = "Image/Jpeg"; // Assuming "Image/Jpeg" is the default image type
            entity.Category = product.Category;
            entity.SubCategoryName = product.subcategoryName;
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = product.CreatedBy;
        }
    }
}
