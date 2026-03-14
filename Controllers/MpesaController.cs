using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.Services.Mpesa;
using Minimart_Api.Services.GuestCheckout;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MpesaController : ControllerBase
    {
        private readonly ILogger<MpesaController> _logger;
        private readonly IMpesaService _mpesaService;
        private readonly IGuestCheckoutService _guestCheckoutService;
        
        public MpesaController(
            ILogger<MpesaController> logger,
            IMpesaService mpesaService,
            IGuestCheckoutService guestCheckoutService)
        {
            _logger = logger;
            _mpesaService = mpesaService;
            _guestCheckoutService = guestCheckoutService;
        }

        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ConfimationRequest request)
        {
            try
            {
                _logger.LogInformation("Received Confirmation Request: {@Request}", request);
                
                // Check if this is a guest checkout by looking at AccountReference
                if (!string.IsNullOrEmpty(request.BillRefNumber))
                {
                    // Try to process as guest checkout first
                    try
                    {
                        var isGuestCheckout = await _guestCheckoutService.ProcessPaymentConfirmationAsync(
                            request.BillRefNumber, 
                            request.TransID);
                            
                        if (isGuestCheckout)
                        {
                            _logger.LogInformation("Guest checkout payment confirmed: {TransID}", request.TransID);
                            return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Not a guest checkout, processing as regular order");
                    }
                }
                
                // Process the confirmation request for registered users here
                var response = _mpesaService.Confirmation(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing M-Pesa confirmation");
                return BadRequest(ex.Message);
            }
            ;
        }

        [HttpPost("validation")]
        public async Task<IActionResult> Validation([FromBody] ValidationRequest request)
        {
            try
            {
                _logger.LogInformation("Received Validation Request: {@Request}", request);
                var response = await _mpesaService.Validation(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in validation");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUrl() {

            try
            {
                var response = await _mpesaService.RegisterUrl();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in registering url");
                return BadRequest(ex.Message);
            }

        }
    }
}
