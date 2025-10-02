using Minimart_Api.DTOS.Mpesa;

namespace Minimart_Api.Repositories.Mpesa
{
    public interface IMpesaRepo
    {
        public Task<ConfirmationResponse> Confirmation(ConfimationRequest request);
        public Task<ValidationResponse> Validation(ValidationRequest request);
    }
}
