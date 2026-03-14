using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.GuestCheckout;
using Minimart_Api.Services.GuestCheckout;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestCartController : ControllerBase
    {
        private readonly IGuestCartService _guestCartService;
        private readonly ILogger<GuestCartController> _logger;

        public GuestCartController(IGuestCartService guestCartService, ILogger<GuestCartController> logger)
        {
            _guestCartService = guestCartService;
            _logger = logger;
        }

        /// <summary>
        /// Add item to guest cart
        /// </summary>
        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] AddToGuestCartDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var result = await _guestCartService.AddToCartAsync(dto);

                return Ok(new ApiResponse<GuestCartResponseDto>
                {
                    Success = true,
                    Message = "Item added to cart successfully",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error adding to guest cart");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to guest cart");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while adding item to cart"
                });
            }
        }

        /// <summary>
        /// Get guest cart by guest ID
        /// </summary>
        [HttpGet("{guestId}")]
        public async Task<IActionResult> GetCart(string guestId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(guestId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Guest ID is required"
                    });
                }

                var result = await _guestCartService.GetCartAsync(guestId);

                return Ok(new ApiResponse<GuestCartResponseDto>
                {
                    Success = true,
                    Message = "Cart retrieved successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving guest cart for {GuestId}", guestId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving cart"
                });
            }
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateGuestCartDto dto, [FromQuery] string guestId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid request data",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                if (string.IsNullOrWhiteSpace(guestId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Guest ID is required"
                    });
                }

                var result = await _guestCartService.UpdateQuantityAsync(dto, guestId);

                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var updatedCart = await _guestCartService.GetCartAsync(guestId);

                return Ok(new ApiResponse<GuestCartResponseDto>
                {
                    Success = true,
                    Message = "Cart updated successfully",
                    Data = updatedCart
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error updating guest cart");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating guest cart item");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating cart"
                });
            }
        }

        /// <summary>
        /// Remove item from guest cart
        /// </summary>
        [HttpDelete("remove/{guestCartId}")]
        public async Task<IActionResult> RemoveItem(int guestCartId, [FromQuery] string guestId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(guestId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Guest ID is required"
                    });
                }

                var result = await _guestCartService.RemoveItemAsync(guestCartId, guestId);

                if (!result)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cart item not found"
                    });
                }

                var updatedCart = await _guestCartService.GetCartAsync(guestId);

                return Ok(new ApiResponse<GuestCartResponseDto>
                {
                    Success = true,
                    Message = "Item removed from cart successfully",
                    Data = updatedCart
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from guest cart");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while removing item from cart"
                });
            }
        }

        /// <summary>
        /// Clear entire guest cart
        /// </summary>
        [HttpDelete("clear/{guestId}")]
        public async Task<IActionResult> ClearCart(string guestId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(guestId))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Guest ID is required"
                    });
                }

                await _guestCartService.ClearCartAsync(guestId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Cart cleared successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing guest cart");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while clearing cart"
                });
            }
        }

        /// <summary>
        /// Admin endpoint: Clean up expired carts
        /// </summary>
        [HttpPost("cleanup")]
        public async Task<IActionResult> CleanupExpiredCarts()
        {
            try
            {
                var count = await _guestCartService.CleanupExpiredCartsAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Cleaned up {count} expired cart items",
                    Data = new { Count = count }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired carts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during cleanup"
                });
            }
        }
    }
}
