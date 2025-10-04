using System.Text;
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
        [Consumes("application/json")]
        public async Task<IActionResult> STKCallback()
        {
            string rawRequestBody = string.Empty;

            try
            {
                // Read the raw request body first
                Request.EnableBuffering(); // This allows us to read the stream multiple times

                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    rawRequestBody = await reader.ReadToEndAsync();
                }

                // Reset the stream position so other middleware can read it
                Request.Body.Position = 0;

                _logger.LogInformation("📥 RAW STK Callback Received: {RawData}", rawRequestBody);

                if (string.IsNullOrWhiteSpace(rawRequestBody))
                {
                    _logger.LogWarning("Empty callback received");
                    return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                }

                // Parse the JSON manually to avoid model binding issues
                var callbackData = JObject.Parse(rawRequestBody);

                // Log the parsed structure for debugging
                _logger.LogInformation("📋 Parsed Callback Structure: {@CallbackData}", callbackData);

                // Extract the stkCallback object - Safaricom uses this structure
                var stkCallback = callbackData["Body"]?["stkCallback"] ?? callbackData["stkCallback"];

                if (stkCallback == null)
                {
                    _logger.LogWarning("❌ No stkCallback found in callback data. Available keys: {Keys}",
                        string.Join(", ", callbackData.Properties().Select(p => p.Name)));
                    return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                }

                var resultCode = stkCallback["ResultCode"]?.Value<int>() ?? -1;
                var resultDesc = stkCallback["ResultDesc"]?.ToString();
                var checkoutRequestId = stkCallback["CheckoutRequestID"]?.ToString();
                var merchantRequestId = stkCallback["MerchantRequestID"]?.ToString();
                var callbackMetadata = stkCallback["CallbackMetadata"];

                _logger.LogInformation("🔍 Callback Processing - ResultCode: {ResultCode}, Desc: {ResultDesc}, CheckoutID: {CheckoutId}",
                    resultCode, resultDesc, checkoutRequestId);

                if (resultCode == 0)
                {
                    // Payment successful - extract transaction details
                    var paymentData = ExtractPaymentDetails(callbackMetadata);

                    _logger.LogInformation($"✅ PAYMENT SUCCESS | Receipt: {paymentData.MpesaReceiptNumber} | Amount: {paymentData.Amount} | Phone: {paymentData.PhoneNumber} | Date: {paymentData.TransactionDate}");

                    // TODO: Save to database
                    await ProcessSuccessfulPayment(paymentData, checkoutRequestId, merchantRequestId);
                }
                else
                {
                    _logger.LogWarning($"❌ PAYMENT FAILED | Code: {resultCode} | Desc: {resultDesc} | CheckoutID: {checkoutRequestId}");

                    // TODO: Handle failed payment
                    await ProcessFailedPayment(checkoutRequestId, merchantRequestId, resultCode, resultDesc);
                }

                // Always return success to Safaricom to prevent retries
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR processing STK Callback. Raw request: {RawRequest}", rawRequestBody);

                // Still return success to Safaricom even if we have processing errors
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
        }


        // Add this private method to your PaymentController
        private PaymentData ExtractPaymentDetails(JToken callbackMetadata)
        {
            var paymentData = new PaymentData();

            if (callbackMetadata?["Item"] is JArray items)
            {
                foreach (var item in items)
                {
                    var name = item["Name"]?.ToString();
                    var value = item["Value"]?.ToString();

                    switch (name)
                    {
                        case "Amount":
                            paymentData.Amount = value;
                            break;
                        case "MpesaReceiptNumber":
                            paymentData.MpesaReceiptNumber = value;
                            break;
                        case "PhoneNumber":
                            paymentData.PhoneNumber = value;
                            break;
                        case "TransactionDate":
                            paymentData.TransactionDate = value;
                            break;
                        case "AccountReference":
                            paymentData.AccountReference = value;
                            break;
                    }
                }
            }

            return paymentData;
        }

        private async Task ProcessSuccessfulPayment(PaymentData paymentData, string checkoutRequestId, string merchantRequestId)
        {
            try
            {
                // TODO: Implement your payment processing logic here
                // Example:
                // await _paymentService.ProcessSuccessfulPaymentAsync(
                //     paymentData.MpesaReceiptNumber,
                //     paymentData.Amount,
                //     paymentData.PhoneNumber,
                //     paymentData.TransactionDate,
                //     checkoutRequestId,
                //     merchantRequestId
                // );

                _logger.LogInformation("💰 Payment processed successfully for Receipt: {Receipt}", paymentData.MpesaReceiptNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving successful payment for Receipt: {Receipt}", paymentData.MpesaReceiptNumber);
            }
        }

        private async Task ProcessFailedPayment(string checkoutRequestId, string merchantRequestId, int resultCode, string resultDesc)
        {
            try
            {
                // TODO: Implement your failed payment handling logic here
                // Example:
                // await _paymentService.ProcessFailedPaymentAsync(checkoutRequestId, merchantRequestId, resultCode, resultDesc);

                _logger.LogInformation("💸 Failed payment recorded for CheckoutID: {CheckoutId}", checkoutRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving failed payment for CheckoutID: {CheckoutId}", checkoutRequestId);
            }
        }

        // Add this class to your DTOS/Mpesa folder or in the same file
        public class PaymentData
        {
            public string Amount { get; set; } = string.Empty;
            public string MpesaReceiptNumber { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string TransactionDate { get; set; } = string.Empty;
            public string AccountReference { get; set; } = string.Empty;
        }



    }
}
