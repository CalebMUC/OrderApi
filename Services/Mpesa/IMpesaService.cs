using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Mpesa;

namespace Minimart_Api.Services.Mpesa
{
    public interface IMpesaService
    {
        public Task<ConfirmationResponse> Confirmation(ConfimationRequest request);
        public Task<ValidationResponse> Validation(ValidationRequest request);
    }
}
