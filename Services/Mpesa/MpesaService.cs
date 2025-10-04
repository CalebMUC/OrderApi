using Minimart_Api.DTOS.Mpesa;
using Minimart_Api.Repositories.Mpesa;

namespace Minimart_Api.Services.Mpesa
{
    public class MpesaService : IMpesaService
    {
        private readonly IMpesaRepo _mpesaRepo;
        public MpesaService(IMpesaRepo mpesaRepo ) {
            _mpesaRepo = mpesaRepo;
        }
        public async Task<ConfirmationResponse> Confirmation(ConfimationRequest request)
        {
            return await _mpesaRepo.Confirmation(request);
        }
        public async Task<ValidationResponse> Validation(ValidationRequest request)
        {
            return await _mpesaRepo.Validation(request);
        }
        public async Task<RegisterUrlResponse> RegisterUrl() {
            return await _mpesaRepo.Register();
        }

        public async Task<StkPushResponse> StkPush(StkPushRequest request) {
            return await _mpesaRepo.StkPush(request);
        }
    }
}
