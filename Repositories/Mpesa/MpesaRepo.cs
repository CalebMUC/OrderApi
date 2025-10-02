using Minimart_Api.Data;
using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.Mpesa
{
    public class MpesaRepo : IMpesaRepo
    {
        private readonly MinimartDBContext _dbContext;
        private readonly ILogger<MpesaRepo> _logger;
        public MpesaRepo(MinimartDBContext dBContext, ILogger<MpesaRepo> logger)
        {
            _dbContext = dBContext;
            _logger = logger;
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
    }
}
