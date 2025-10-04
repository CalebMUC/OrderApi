using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.Services.Mpesa;
using Newtonsoft.Json.Linq;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IMpesaService _mpesaService;
        public PaymentController(ILogger<PaymentController> logger, IMpesaService mpesaService) {
            _logger = logger;
            _mpesaService = mpesaService;
        }

        [HttpPost("confirmation")]
        public async Task<IActionResult> Confirmation([FromBody] ConfimationRequest request)
        {
            try
            {
                _logger.LogInformation("Received Confirmation Request: {@Request}", request);
                // Process the confirmation request here
                var response = _mpesaService.Confirmation(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
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
        public async Task<IActionResult> RegisterUrl()
        {

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

        [HttpPost("stkPush")]
        public async Task<IActionResult> StkPush([FromBody] StkPushRequest request)
        {
            try
            {
                _logger.LogInformation("Received STK Push Request: {@Request}", request);
                var response = await _mpesaService.StkPush(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in STK Push");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("stkcallback")]
        public async Task<IActionResult> STKCallback([FromBody] StkCallbackRequest callbackRequest)
        {
            try
            {
                _logger.LogInformation("STK Callback Received: {@Callback}", callbackRequest);

                if (callbackRequest?.Body?.StkCallback == null)
                {
                    _logger.LogWarning("Invalid callback structure");
                    return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                }

                var callback = callbackRequest.Body.StkCallback;

                if (callback.ResultCode == 0)
                {
                    // Process successful payment
                    var amount = callback.CallbackMetadata?.Item?.FirstOrDefault(x => x.Name == "Amount")?.Value?.ToString();
                    var mpesaReceipt = callback.CallbackMetadata?.Item?.FirstOrDefault(x => x.Name == "MpesaReceiptNumber")?.Value?.ToString();
                    var phone = callback.CallbackMetadata?.Item?.FirstOrDefault(x => x.Name == "PhoneNumber")?.Value?.ToString();

                    _logger.LogInformation($"✅ Payment Success | Receipt: {mpesaReceipt} | Amount: {amount} | Phone: {phone}");
                }
                else
                {
                    _logger.LogWarning($"❌ Payment Failed | Code: {callback.ResultCode} | Desc: {callback.ResultDesc}");
                }

                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing STK Callback");
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
        }



    }
}
