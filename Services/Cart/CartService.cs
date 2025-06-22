using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Repositories.Cart;
using Minimart_Api.Models;
using Minimart_Api.Repositories.ProductRepository;

namespace Minimart_Api.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepo _cartRepo;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(ICartRepo cartRepo,IProductRepository productRepository, ILogger<CartService> logger ) { 
            _cartRepo = cartRepo;
            _productRepository = productRepository;
            _logger = logger;
        }


        public async Task<Status> AddToCart(string CartItems)
        {
            return await _cartRepo.AddToCart(CartItems);
        }


        public async Task<Status> DeleteCartItems(CartItemsDTO CartItems)
        {
            return await _cartRepo.DeleteCartItems(CartItems);
        }

        //public async Task<Status> SaveItems(SaveItemsDTO saveItems)
        //{
        //    return await _cartRepo.SaveItems(saveItems);
        //}

        public async Task<IEnumerable<CartResults>> GetCartItems(int UserID)
        {
            return await _cartRepo.GetCartItems(UserID);
        }

        public async Task<IEnumerable<CartResults>> GetBoughtItems(int UserID)
        {
            return await _cartRepo.GetBoughtItems(UserID);
        }

        //public async Task<IEnumerable<Products>> GetSavedItems()
        //{
        //    return await _cartRepo.GetSavedItems();
        //}

        public async Task<SavedProductsDto> SaveItemAsync(SaveItemDto itemDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null)
                {
                    throw new ArgumentException("Product not found");
                }

                var savedItem = new SavedItems
                {
                    UserId = itemDto.UserId,
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity,
                    SavedOn = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _cartRepo.SaveItemAsync(savedItem);
                return MapToDto(result, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving item for user {UserId}", itemDto.UserId);
                throw;
            }
        }

        public async Task<bool> RemoveItemAsync(int userId, string productId)
        {
            try
            {
                return await _cartRepo.RemoveItemAsync(userId, productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing saved item for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<SavedProductsDto>> GetSavedItemsAsync(int userId)
        {
            try
            {
                var savedItems = await _cartRepo.GetSavedItemsAsync(userId);
                var productIds = savedItems.Select(s => s.ProductId).ToList();
                var products = await _productRepository.GetProductsByIdsAsync(productIds);

                return savedItems.Select(si =>
                {
                    var product = products.FirstOrDefault(p => p.ProductId == si.ProductId);
                    return MapToDto(si, product);
                }).Where(dto => dto != null).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved items for user {UserId}", userId);
                throw;
            }
        }

        private SavedProductsDto MapToDto(SavedItems savedItem, Products product)
        {
            if (product == null) return null;

            return new SavedProductsDto
            {
                ProductID = product.ProductId,
                ProductName = product.ProductName,
                ProductImage = product.ImageUrl,
                Price = product.Price ?? 0,
                Discount = product.Discount,
                InStock = product.InStock,
                CategoryName = product.CategoryName,
                SavedOn = savedItem.SavedOn,
                Quantity = savedItem.Quantity
            };
        }


    }
}
