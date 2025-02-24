using Minimart_Api.DTOS;
using Minimart_Api.Repositories.SystemMerchantsRepository;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services.SystemMerchantService
{
    public class MerchantsService : ISystemMerchants
    {
        private readonly ISystemMerchantRepo _systemMerchantRepo;
        public MerchantsService(ISystemMerchantRepo systemMerchantRepo) {
            _systemMerchantRepo = systemMerchantRepo;
        }

        public async Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync() { 

            return await _systemMerchantRepo.GetAllMerchantsAsync();
        }

        public async Task<SystemMerchants> GetMerchantByIdAsync(int merchantId)
        {

            return await _systemMerchantRepo.GetMerchantByIdAsync(merchantId);
        }

        public async Task<ResponseStatus> AddMerchantsAsync(SystemMerchantsDto merchant) {

            return await _systemMerchantRepo.AddMerchantsAsync(merchant);

        }

        public async Task<ResponseStatus> UpdateMerchantsAsync(SystemMerchantsDto merchant)
        {

            return await _systemMerchantRepo.UpdateMerchantsAsync(merchant);

        }
        public async Task<ResponseStatus> DeleteMerchantAsync(int merchantId) {

            return await _systemMerchantRepo.DeleteMerchantAsync(merchantId);
        }
    }

}
