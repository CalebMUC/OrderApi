using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.Models;
using Newtonsoft.Json;

namespace Minimart_Api.Repositories.Mpesa
{
    public class MpesaRepo : IMpesaRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<MpesaRepo> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly MpesaGoLive mpesaGoLive;
        public MpesaRepo(MinimartDBContext dBContext, ILogger<MpesaRepo> logger,
            IHttpClientFactory clientFactory,IOptions<MpesaGoLive> options )
        {
            _dbContext = dBContext;
            _logger = logger;
            _clientFactory = clientFactory;
            mpesaGoLive = options.Value;
        }
        public async Task<ConfirmationResponse> Confirmation(ConfimationRequest request)
        {
            try
            {
                _logger.LogInformation("Processing Confirmation Request: {@Request}", request);
                // Implement your logic to handle the confirmation request here

                var transaction = new MpesaTransaction
                {
                    TransactionType = request.TransactionType,
                    TransID = request.TransID,
                    TransTime = request.TransTime,
                    TransAmount = request.TransAmount,
                    BusinessShortCode = request.BusinessShortCode,
                    BillRefNumber = request.BillRefNumber,
                    InvoiceNumber = request.InvoiceNumber,
                    OrgAccountBalance = request.OrgAccountBalance,
                    ThirdPartyTransID = request.ThirdPartyTransID,
                    MSISDN = request.MSISDN,
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName
                };

                _dbContext.MpesaTransactions.Add(transaction);
                await _dbContext.SaveChangesAsync();

                var response = new ConfirmationResponse
                {
                    ResultCode = 0,
                    ResultDesc = "Confirmation received successfully"
                };
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing confirmation request: {@Request}", request);
                throw;
            }
        }
        public async Task<ValidationResponse> Validation(ValidationRequest request)
        {
            _logger.LogInformation("Processing Validation: {@Request}", request);

            // Example rule: reject transactions less than 1 shilling
            if (decimal.TryParse(request.TransAmount, out decimal amount) && amount < 1)
            {
                return new ValidationResponse
                {
                    ResultCode = 1,
                    ResultDesc = "Transaction amount too low"
                };
            }

            return new ValidationResponse
            {
                ResultCode = 0,
                ResultDesc = "Transaction validated successfully"
            };
        }

        public async Task<RegisterUrlResponse> Register()
        {
            string accessToken = await GetAccessTokenAsync();
            try
            {
                var client = _clientFactory.CreateClient();
                client.BaseAddress = new Uri("https://api.safaricom.co.ke/");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var registerUrlRequest = new 
                {
                    ShortCode = mpesaGoLive.ShortCode,
                    ResponseType = "Completed",
                    ConfirmationURL = mpesaGoLive.ConfirmationUrl,
                    ValidationURL = mpesaGoLive.ValidationUrl
                };
                var jsonContent = JsonConvert.SerializeObject(registerUrlRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("mpesa/c2b/v1/registerurl", content);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var registerUrlResponse = JsonConvert.DeserializeObject<RegisterUrlResponse>(jsonResponse);
                    return registerUrlResponse;
                }
                else
                {
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to register URL. Status: {Status}, Response: {Response}", response.StatusCode, errorResponse);
                    throw new Exception($"Failed to register URL. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error registering URL");
                throw;
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://api.safaricom.co.ke/");

            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{mpesaGoLive.ConsumerKey}:{mpesaGoLive.ConsumerSecret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            HttpResponseMessage response = await client.GetAsync("oauth/v1/generate?grant_type=client_credentials");

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                dynamic tokenResponse = JsonConvert.DeserializeObject(jsonResponse);
                return tokenResponse.access_token;
            }
            else
            {
                throw new Exception($"Failed to get access token. Status: {response.StatusCode}");
            }
        }
    }
}
