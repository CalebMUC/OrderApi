using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;
using OpenSearch.Client;

namespace Minimart_Api.Repositories.Cart
{
    public class CartRepo : ICartRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<Categories> _logger;

        public CartRepo(MinimartDBContext dBContext, ILogger<Categories> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int userId)
        {
            return await _dbContext.CartItems.Where(ci => ci.Cart.UserId == userId && ci.IsActive == true)
                .Select(ci => new CartResults
                {
                    productID = ci.ProductId,
                    ProductImage = ci.Products.ImageUrl,
                    ProductName = ci.Products.ProductName,
                    MerchantId = ci.Products.MerchantID,
                    Quantity = ci.Quantity,
                    price = ci.Products.Price,
                    Instock = ci.Products.StockQuantity,
                    ProductDescription = ci.Products.ProductDescription,
                    KeyFeatures = ci.Products.KeyFeatures,
                    Specification = ci.Products.Specification,
                    Box = ci.Products.Box,
                    CartID = ci.CartId ?? 0,
                    CartItemID = ci.CartItemId,
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<CartResults>> GetBoughtItems(int userId)
        {
            return await _dbContext.CartItems.Where(ci => ci.Cart.UserId == userId && ci.IsBought == true)
                .Select(ci => new CartResults
                {
                    productID = ci.Products.ProductId,
                    ProductImage = ci.Products.ImageUrl,
                    ProductName = ci.Products.ProductName,
                    MerchantId = ci.Products.MerchantID,
                    Quantity = ci.Quantity,
                    price = ci.Products.Price,
                    Instock = ci.Products.StockQuantity,
                    ProductDescription = ci.Products.ProductDescription,
                    KeyFeatures = ci.Products.KeyFeatures,
                    Specification = ci.Products.Specification,
                    Box = ci.Products.Box,
                    CartID = ci.CartId ?? 0,
                    CartItemID = ci.CartItemId,
                })
                .ToListAsync();
        }


        public async Task<Status> AddToCart(string cartItemsJson)
        {
            // Parse the JSON input
            var json = JsonDocument.Parse(cartItemsJson);
            var userId = json.RootElement.GetProperty("UserID").GetInt32();
            var productId = json.RootElement.GetProperty("ProductID").GetString();
            var quantity = json.RootElement.GetProperty("Quantity").GetInt32();

            // Fetch the user
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return new Status { ResponseCode = 400, ResponseMessage = "User not found" };
            }

            // Check if product exists
            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                return new Status { ResponseCode = 400, ResponseMessage = "Product Doesn't Exist" };
            }

            // Get or create the cart for the user
            var cart = await _dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    CartName = user.UserName
                };
                _dbContext.Cart.Add(cart);
                await _dbContext.SaveChangesAsync(); // Save to get CartID
            }

            // Now check if item is already in the cart
            var existingCartItem = await _dbContext.CartItems
                .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.ProductId == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity = quantity;
                existingCartItem.UpdatedOn = DateTime.Now;

                await _dbContext.SaveChangesAsync();
                return new Status { ResponseCode = 200, ResponseMessage = "Product Updated in cart Successfully" };
            }
            else
            {
                var newCartItem = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = quantity,
                    CreatedOn = DateTime.Now
                };

                _dbContext.CartItems.Add(newCartItem);
                await _dbContext.SaveChangesAsync();
                return new Status { ResponseCode = 200, ResponseMessage = "Product Added to Cart Successfully" };
            }
        }



        //public async Task<Status> SaveItems(SaveItemsDTO saveItems)
        //{
        //    try
        //    {
        //        var item = _dbContext.Products.FirstOrDefault(c =>
        //            c.ProductId == saveItems.ProductID);

        //        // Check if the item exists
        //        if (item == null)
        //        {
        //            return new Status
        //            {
        //                ResponseCode = 404,
        //                ResponseMessage = "Item not found"
        //            };
        //        }

        //        // Handle null properties
        //        if (item.IsSaved == null)
        //        {
        //            item.IsSaved = 0; // Set default value
        //        }
        //        else
        //        {
        //            item.IsSaved = 1; // Update value
        //            //Exclude RowID Explicily to prevent it from beind update
        //            _dbContext.Entry(item).Property(x => x.RowId).IsModified = false;
        //            await _dbContext.SaveChangesAsync();
        //        }

        //        //_dbContext.TProducts.Update(item);
        //        //await _dbContext.SaveChangesAsync();

        //        return new Status
        //        {
        //            ResponseCode = 200,
        //            ResponseMessage = "Item Saved Successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Status
        //        {
        //            ResponseCode = 500,
        //            ResponseMessage = ex.Message
        //        };
        //    }
        //}

        //public async Task<IEnumerable<TProduct>> GetSavedItems()
        //{
        //    return await _dbContext.TProducts.Where(P => P.IsSaved == 1)
        //        .Select(ci => new TProduct
        //        {
        //            ProductId = ci.ProductId,
        //            ImageUrl = ci.ImageUrl,
        //            ProductName = ci.ProductName,
        //            //Quantity = ci.InStock,
        //            Price = ci.Price,
        //            InStock = ci.InStock,
        //            ProductDescription = ci.ProductDescription,
        //            KeyFeatures = ci.KeyFeatures,
        //            Specification = ci.Specification,
        //            Box = ci.Box
        //        })
        //        .ToListAsync();
        //}

        public async Task<SavedItems> SaveItemAsync(SavedItems item)
        {
            // Check if item already exists
            var existingItem = await _dbContext.SavedItems
                .FirstOrDefaultAsync(s => s.UserId == item.UserId && s.ProductId == item.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity = item.Quantity;
                existingItem.IsActive = true;
                existingItem.SavedOn = DateTime.UtcNow;
            }
            else
            {
                _dbContext.SavedItems.Add(item);
            }

            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task<bool> RemoveItemAsync(int userId, string productId)
        {
            var item = await _dbContext.SavedItems
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId);

            if (item != null)
            {
                item.IsActive = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<SavedItems>> GetSavedItemsAsync(int userId)
        {
            return await _dbContext.SavedItems
                .Include(s => s.Products)
                .Where(s => s.UserId == userId && s.IsActive)
                .OrderByDescending(s => s.SavedOn)
                .ToListAsync();
        }

        public async Task<SavedItems> GetSavedItemAsync(int userId, string productId)
        {
            return await _dbContext.SavedItems
                .FirstOrDefaultAsync(s => s.UserId == userId && s.ProductId == productId && s.IsActive);
        }


        public async Task<Status> DeleteCartItems(CartItemsDTO cartItems)
        {
            try
            {

                var item = _dbContext.CartItems.FirstOrDefault(c =>
                c.CartItemId == cartItems.CartItemID &&
                c.CartId == cartItems.CartID &&
                c.ProductId == cartItems.ProductID);

                //check if the item Exists
                if (item != null)
                {
                    _dbContext.CartItems.Remove(item);
                    _dbContext.SaveChanges();
                }

                var response = new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Item Removed Successfully"
                };

                return response;


            }
            catch (Exception ex)
            {
                var response = new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = ex.Message
                };

                return response;
            }


        }
    }
}
