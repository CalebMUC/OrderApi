using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.GuestCheckout;
using Minimart_Api.Models;
using Minimart_Api.Utilities;

namespace Minimart_Api.Services.GuestCheckout
{
    public class GuestCartService : IGuestCartService
    {
        private readonly MinimartDBContext _context;
        private readonly ILogger<GuestCartService> _logger;

        public GuestCartService(MinimartDBContext context, ILogger<GuestCartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GuestCartResponseDto> AddToCartAsync(AddToGuestCartDto dto)
        {
            try
            {
                // Validate product exists and is active
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.IsActive && !p.IsDeleted);

                if (product == null)
                {
                    throw new ArgumentException("Product not found or inactive");
                }

                // Check if product has sufficient stock
                if (product.StockQuantity < dto.Quantity)
                {
                    throw new ArgumentException($"Insufficient stock. Available: {product.StockQuantity}");
                }

                // Check if item already exists in cart
                var existingItem = await _context.GuestCarts
                    .FirstOrDefaultAsync(gc => gc.GuestId == dto.GuestId && gc.ProductId == dto.ProductId);

                if (existingItem != null)
                {
                    // Update quantity
                    existingItem.Quantity += dto.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    existingItem.ExpiresAt = DateTime.UtcNow.AddDays(7); // Extend expiration
                }
                else
                {
                    // Add new item
                    var cartItem = new GuestCart
                    {
                        GuestId = dto.GuestId,
                        ProductId = dto.ProductId,
                        Quantity = dto.Quantity,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        ExpiresAt = DateTime.UtcNow.AddDays(7)
                    };

                    _context.GuestCarts.Add(cartItem);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Guest {GuestId} added product {ProductId} to cart", dto.GuestId, dto.ProductId);

                return await GetCartAsync(dto.GuestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to guest cart for guest {GuestId}", dto.GuestId);
                throw;
            }
        }

        public async Task<GuestCartResponseDto> GetCartAsync(string guestId)
        {
            try
            {
                var cartItems = await _context.GuestCarts
                    .Include(gc => gc.Product)
                    .Where(gc => gc.GuestId == guestId && gc.ExpiresAt > DateTime.UtcNow)
                    .ToListAsync();

                var items = cartItems.Select(gc =>
                {
                    // ? Calculate discounted price using utility
                    var discountedPrice = PriceUtility.CalculateDiscountedPrice(
                        gc.Product.Price,
                        gc.Product.Discount
                    );

                    return new GuestCartItemDto
                    {
                        GuestCartId = gc.GuestCartId,
                        ProductId = gc.Product.ProductId,
                        ProductName = gc.Product.ProductName,
                        ProductDescription = gc.Product.Description,
                        ProductImage = gc.Product.ImageUrls?.FirstOrDefault(),
                        Price = gc.Product.Price,
                        Discount = gc.Product.Discount,
                        DiscountedPrice = discountedPrice,
                        Quantity = gc.Quantity,
                        StockQuantity = gc.Product.StockQuantity,
                        IsActive = gc.Product.IsActive,
                        ItemTotal = PriceUtility.RoundPrice(discountedPrice * gc.Quantity),
                        CreatedAt = gc.CreatedAt,
                        ExpiresAt = gc.ExpiresAt
                    };
                }).ToList();

                var response = new GuestCartResponseDto
                {
                    GuestId = guestId,
                    Items = items,
                    TotalItems = items.Sum(i => i.Quantity),
                    Subtotal = PriceUtility.RoundPrice(items.Sum(i => i.ItemTotal)),
                    ExpiresAt = items.Any() ? items.Min(i => i.ExpiresAt) : null
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guest cart for guest {GuestId}", guestId);
                throw;
            }
        }

        public async Task<bool> UpdateQuantityAsync(UpdateGuestCartDto dto, string guestId)
        {
            try
            {
                var cartItem = await _context.GuestCarts
                    .Include(gc => gc.Product)
                    .FirstOrDefaultAsync(gc => gc.GuestCartId == dto.GuestCartId && gc.GuestId == guestId);

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {GuestCartId} not found for guest {GuestId}", dto.GuestCartId, guestId);
                    return false;
                }

                // Check stock availability
                if (cartItem.Product.StockQuantity < dto.Quantity)
                {
                    throw new ArgumentException($"Insufficient stock. Available: {cartItem.Product.StockQuantity}");
                }

                cartItem.Quantity = dto.Quantity;
                cartItem.UpdatedAt = DateTime.UtcNow;
                cartItem.ExpiresAt = DateTime.UtcNow.AddDays(7); // Extend expiration

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated cart item {GuestCartId} quantity to {Quantity} for guest {GuestId}", 
                    dto.GuestCartId, dto.Quantity, guestId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item {GuestCartId} for guest {GuestId}", dto.GuestCartId, guestId);
                throw;
            }
        }

        public async Task<bool> RemoveItemAsync(int guestCartId, string guestId)
        {
            try
            {
                var cartItem = await _context.GuestCarts
                    .FirstOrDefaultAsync(gc => gc.GuestCartId == guestCartId && gc.GuestId == guestId);

                if (cartItem == null)
                {
                    _logger.LogWarning("Cart item {GuestCartId} not found for guest {GuestId}", guestCartId, guestId);
                    return false;
                }

                _context.GuestCarts.Remove(cartItem);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Removed cart item {GuestCartId} for guest {GuestId}", guestCartId, guestId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item {GuestCartId} for guest {GuestId}", guestCartId, guestId);
                throw;
            }
        }

        public async Task<bool> ClearCartAsync(string guestId)
        {
            try
            {
                var cartItems = await _context.GuestCarts
                    .Where(gc => gc.GuestId == guestId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return true;
                }

                _context.GuestCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleared cart for guest {GuestId}, removed {Count} items", guestId, cartItems.Count);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart for guest {GuestId}", guestId);
                throw;
            }
        }

        public async Task<int> CleanupExpiredCartsAsync()
        {
            try
            {
                var expiredItems = await _context.GuestCarts
                    .Where(gc => gc.ExpiresAt <= DateTime.UtcNow)
                    .ToListAsync();

                if (!expiredItems.Any())
                {
                    return 0;
                }

                _context.GuestCarts.RemoveRange(expiredItems);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Cleaned up {Count} expired guest cart items", expiredItems.Count);

                return expiredItems.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired guest carts");
                throw;
            }
        }
    }
}
