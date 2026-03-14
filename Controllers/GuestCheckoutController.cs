using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.GuestCheckout;
using Minimart_Api.Services.GuestCheckout;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuestCheckoutController : ControllerBase
    {
        private readonly IGuestCheckoutService _guestCheckoutService;
        private readonly ILogger<GuestCheckoutController> _logger;

        public GuestCheckoutController(
            IGuestCheckoutService guestCheckoutService,
            ILogger<GuestCheckoutController> logger)
        {
            _guestCheckoutService = guestCheckoutService;
            _logger = logger;
        }

        /// <summary>
        /// Initiate guest checkout and M-Pesa STK Push
        /// </summary>
        [HttpPost("initiate")]
        public async Task<IActionResult> InitiateCheckout([FromBody] InitiateGuestCheckoutDto dto)
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

                var result = await _guestCheckoutService.InitiateCheckoutAsync(dto);

                return Ok(new ApiResponse<GuestCheckoutResponseDto>
                {
                    Success = true,
                    Message = "Checkout initiated successfully. Please complete payment on your phone.",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error during guest checkout");
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating guest checkout");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while processing your checkout. Please try again."
                });
            }
        }

        /// <summary>
        /// Get checkout status
        /// </summary>
        [HttpGet("status/{guestCheckoutId}")]
        public async Task<IActionResult> GetCheckoutStatus(Guid guestCheckoutId)
        {
            try
            {
                var result = await _guestCheckoutService.GetCheckoutStatusAsync(guestCheckoutId);

                return Ok(new ApiResponse<GuestCheckoutStatusDto>
                {
                    Success = true,
                    Message = "Checkout status retrieved successfully",
                    Data = result
                });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Checkout not found: {GuestCheckoutId}", guestCheckoutId);
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving checkout status");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving checkout status"
                });
            }
        }

        /// <summary>
        /// Calculate delivery fee before checkout
        /// </summary>
        [HttpPost("calculate-delivery")]
        public async Task<IActionResult> CalculateDelivery([FromBody] DeliveryCalculationDto dto)
        {
            try
            {
                var deliveryFee = await _guestCheckoutService.CalculateDeliveryFeeAsync(
                    dto.CountyId,
                    dto.TownId,
                    dto.DeliveryStationId
                );

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Delivery fee calculated successfully",
                    Data = new { DeliveryFee = deliveryFee }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating delivery fee");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while calculating delivery fee"
                });
            }
        }

        /// <summary>
        /// Admin endpoint: Abandon old pending checkouts
        /// </summary>
        [HttpPost("abandon-old")]
        public async Task<IActionResult> AbandonOldCheckouts()
        {
            try
            {
                var count = await _guestCheckoutService.AbandonOldPendingCheckoutsAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Abandoned {count} old pending checkouts",
                    Data = new { Count = count }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error abandoning old checkouts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during cleanup"
                });
            }
        }
    }

    public class DeliveryCalculationDto
    {
        public int? CountyId { get; set; }
        public int? TownId { get; set; }
        public int? DeliveryStationId { get; set; }
    }
}
