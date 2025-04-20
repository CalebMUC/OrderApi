using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.ProductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MinimartDBContext _dbContext;
        public ProductRepository(MinimartDBContext dBContext)
        {
            _dbContext = dBContext;

        }
        public async Task<IEnumerable<Products>> GetAllProducts()
        {
            return await _dbContext.Products.ToListAsync();

        }

        public async Task<Status> EditProductsAsync(AddProducts products)
        {
            try
            {
                var existingProduct = await _dbContext.Products.FindAsync(products.productID);
                updateProductFromDto(existingProduct, products);
                await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Products Edit Successfully"
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = $"Products failed to  Edited {ex.Message} "
                };

            }
        }

        public async Task<Status> AddProducts(AddProducts product)
        {

            // Check if the product already exists
            var existingProduct = await _dbContext.Products
                .FirstOrDefaultAsync(x => x.ProductName == product.productName);
            if (existingProduct != null)
            {
                return new Status
                {
                    ResponseCode = 409,
                    ResponseMessage = $"Product '{product.productName}' Already Exists"
                };
            }

            // Create a new product entity
            var newProduct = new Products
            {
                MerchantID = product.merchantID,
                ProductId = product.productID,
                ProductName = product.productName,
                ProductDescription = product.productDetails,
                ProductType = "P",
                CategoryId = product.CategoryID,
                SearchKeyWord = product.SearchKeyWord,
                Price = product.price,
                InStock = product.InStock,
                StockQuantity = product.Quantity,
                Discount = product.discount,
                ImageUrl = product.productImage,
                KeyFeatures = product.productFeatures,
                Box = product.boxContent,
                Specification = product.productSpecifications,
                CreatedOn = DateTime.Now,
                CreatedBy = product.CreatedBy,
                ImageType = "Image/Jpeg",
                //Category = product.Category,
                //SubCategoryName = product.subcategoryName

            };

            // Add the new product to the database
            _dbContext.Products.Add(newProduct);

            try
            {
                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Product Added Successfully"
                };
            }
            catch (Exception ex)
            {
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Internal Server Error: " + ex.Message
                };
            }
        }


        //update existing Entity from DTO
        public void updateProductFromDto(Products entity, AddProducts product)
        {
            // Update the product entity with values from the DTO
           entity.MerchantID = product.merchantID;
            entity.ProductId = product.productID;
            entity.ProductName = product.productName;
            entity.ProductDescription = product.productDetails;
            entity.ProductType = "P"; // Assuming "P" is the default product type
            entity.CategoryId = product.CategoryID;
            entity.SearchKeyWord = product.SearchKeyWord;
            entity.Price = product.price;
            entity.StockQuantity = product.Quantity;
            entity.Discount = product.discount;
            entity.ImageUrl = product.productImage;
            entity.KeyFeatures = product.productFeatures;
            entity.Box = product.boxContent;
            entity.Specification = product.productSpecifications;
            entity.CreatedOn = DateTime.Now; // Update the creation timestamp
            entity.CreatedBy = product.CreatedBy;
            entity.ImageType = "Image/Jpeg"; // Assuming "Image/Jpeg" is the default image type
            entity.CategoryName = product.CategoryName;
            entity.InStock = product.InStock;
            //entity.SubCategoryName = product.subcategoryName;
            entity.UpdatedOn = DateTime.Now;
            entity.UpdatedBy = product.CreatedBy;
        }



        public async Task<IEnumerable<Products>> FetchAllProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<IEnumerable<Products>> LoadProductImages(string productId)
        {
            return await _dbContext.Products
                .Where(w => w.ProductId == productId.ToString())
                .ToListAsync();
        }


        public async Task<IEnumerable<CartResults>> GetProductsByCategory(int categoryId)
        {
            return await _dbContext.Products
                .Where(tp => tp.CategoryId == categoryId)
                .Select(tp => new CartResults
                {
                    productID = tp.ProductId,
                    ProductName = tp.ProductName,
                    ProductImage = tp.ImageUrl,
                    Instock = tp.StockQuantity,
                    price = tp.Price,
                })
                .ToListAsync();
        }
    }
}
