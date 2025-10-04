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
            _logger.LogInformation("STK Callback Received: {Data}", callbackData?.ToString());

            try
            {
                // Safaricom callback structure: { "Body": { "stkCallback": { ... } } }
                var stkCallback = callbackData?["Body"]?["stkCallback"];

                if (stkCallback == null)
                {
                    _logger.LogWarning("No stkCallback found in callback data");
                    return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                }

                var resultCode = stkCallback["ResultCode"]?.Value<int>() ?? -1;
                var resultDesc = stkCallback["ResultDesc"]?.ToString();
                var checkoutRequestId = stkCallback["CheckoutRequestID"]?.ToString();
                var callbackMetadata = stkCallback["CallbackMetadata"];

                _logger.LogInformation("Callback Processing - ResultCode: {ResultCode}, Desc: {ResultDesc}", resultCode, resultDesc);

                if (resultCode == 0)
                {
                    // Payment successful
                    string amount = "";
                    string mpesaReceipt = "";
                    string phone = "";
                    string transactionDate = "";

                    if (callbackMetadata?["Item"] is JArray items)
                    {
                        foreach (var item in items)
                        {
                            var name = item["Name"]?.ToString();
                            var value = item["Value"]?.ToString();

                            switch (name)
                            {
                                case "Amount":
                                    amount = value;
                                    break;
                                case "MpesaReceiptNumber":
                                    mpesaReceipt = value;
                                    break;
                                case "PhoneNumber":
                                    phone = value;
                                    break;
                                case "TransactionDate":
                                    transactionDate = value;
                                    break;
                            }
                        }
                    }

                    _logger.LogInformation($"✅ Payment Success | Receipt: {mpesaReceipt} | Amount: {amount} | Phone: {phone}");

                    // TODO: Save to database
                    // await _paymentService.ProcessSuccessfulPayment(mpesaReceipt, amount, phone, transactionDate, checkoutRequestId);
                }
                else
                {
                    _logger.LogWarning($"❌ Payment Failed | Code: {resultCode} | Desc: {resultDesc}");

                    // TODO: Handle failed payment
                    // await _paymentService.ProcessFailedPayment(checkoutRequestId, resultCode.ToString(), resultDesc);
                }

                // Always return success to Safaricom
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
