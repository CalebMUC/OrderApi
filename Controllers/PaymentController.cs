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
        public async Task<IActionResult> STKCallback([FromBody] JObject callbackData)
        {
            _logger.LogInformation("STK Callback Received: {Data}", callbackData.ToString());

            try
            {
                var resultCode = callbackData["Body"]?["stkCallback"]?["ResultCode"]?.ToString();
                var resultDesc = callbackData["Body"]?["stkCallback"]?["ResultDesc"]?.ToString();
                var checkoutRequestId = callbackData["Body"]?["stkCallback"]?["CheckoutRequestID"]?.ToString();
                var metadata = callbackData["Body"]?["stkCallback"]?["CallbackMetadata"];

                if (resultCode == "0")
                {
                    // Payment successful — extract transaction details
                    string amount = metadata?["Item"]?.FirstOrDefault(i => i["Name"]?.ToString() == "Amount")?["Value"]?.ToString();
                    string mpesaReceipt = metadata?["Item"]?.FirstOrDefault(i => i["Name"]?.ToString() == "MpesaReceiptNumber")?["Value"]?.ToString();
                    string phone = metadata?["Item"]?.FirstOrDefault(i => i["Name"]?.ToString() == "PhoneNumber")?["Value"]?.ToString();

                    // Save to database
                    _logger.LogInformation($"✅ Payment Success | Receipt: {mpesaReceipt} | Amount: {amount} | Phone: {phone}");
                }
                else
                {
                    _logger.LogWarning($"❌ STK Push Failed | ResultCode: {resultCode} | Desc: {resultDesc}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing STK Callback");
                return BadRequest();
            }
        }



    }
}
