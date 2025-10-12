using System.Text;
using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.DTOS.Payments;
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

        [HttpPost("query-status")]
        public async Task<IActionResult> TrxQueryStatus([FromBody] MpesaTrxQuery query)
        {
            try
            {
                _logger.LogInformation("Received STK Push Request: {@Request}", query);
                var response = await _mpesaService.TrxQueryStatus(query); 
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
                Request.EnableBuffering();

                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    rawRequestBody = await reader.ReadToEndAsync();
                }
                Request.Body.Position = 0;

                _logger.LogInformation("📥 RAW STK Callback Received: {RawData}", rawRequestBody);

                if (string.IsNullOrWhiteSpace(rawRequestBody))
                {
                    _logger.LogWarning("Empty callback received");
                    return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                }

                var callbackData = JObject.Parse(rawRequestBody);
                _logger.LogInformation("📋 Parsed Callback Structure: {@CallbackData}", callbackData);

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

                    // Validate extracted data
                    if (string.IsNullOrWhiteSpace(paymentData.MpesaReceiptNumber) ||
                        string.IsNullOrWhiteSpace(paymentData.Amount) ||
                        string.IsNullOrWhiteSpace(paymentData.PhoneNumber))
                    {
                        _logger.LogError("Missing required payment data: {@PaymentData}", paymentData);
                        return Ok(new { ResultCode = 0, ResultDesc = "Success" });
                    }

                    _logger.LogInformation("✅ PAYMENT SUCCESS | Receipt: {Receipt} | Amount: {Amount} | Phone: {Phone} | Date: {Date}",
                        paymentData.MpesaReceiptNumber, paymentData.Amount, paymentData.PhoneNumber, paymentData.TransactionDate);

                    // Save to database via service
                    var processed = await _mpesaService.ProcessSuccessfulPayment(paymentData, checkoutRequestId, merchantRequestId);
                    if (!processed)
                    {
                        _logger.LogError("Failed to process successful payment for CheckoutRequestID: {CheckoutRequestId}", checkoutRequestId);
                    }
                }
                else
                {
                    _logger.LogWarning("❌ PAYMENT FAILED | Code: {ResultCode} | Desc: {ResultDesc} | CheckoutID: {CheckoutId}",
                        resultCode, resultDesc, checkoutRequestId);

                    await ProcessFailedPayment(checkoutRequestId, merchantRequestId, resultCode, resultDesc);
                }

                // Always return success to Safaricom to prevent retries
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 ERROR processing STK Callback. Raw request: {RawRequest}", rawRequestBody);
                // Always return success to Safaricom to prevent retries
                return Ok(new { ResultCode = 0, ResultDesc = "Success" });
            }
        }

        // Improved extraction with null/format handling
        private PaymentData ExtractPaymentDetails(JToken callbackMetadata)
        {
            var paymentData = new PaymentData();

            if (callbackMetadata?["Item"] is JArray items)
            {
                foreach (var item in items)
                {
                    var name = item["Name"]?.ToString();
                    var valueToken = item["Value"];
                    var value = valueToken?.ToString();

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

        private async Task ProcessFailedPayment(string checkoutRequestId, string merchantRequestId, int resultCode, string resultDesc)
        {
            try
            {
                // TODO: Implement your failed payment handling logic here
                _logger.LogInformation("💸 Failed payment recorded for CheckoutID: {CheckoutId}", checkoutRequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving failed payment for CheckoutID: {CheckoutId}", checkoutRequestId);
            }
        }



    }
}
