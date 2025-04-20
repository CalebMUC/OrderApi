using Minimart_Api.DTOS.Merchants;
using Minimart_Api.Repositories.SystemMerchantsRepository;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;

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

        public async Task<Status> AddMerchantsAsync(SystemMerchantsDto merchant) {

            return await _systemMerchantRepo.AddMerchantsAsync(merchant);

        }

        public async Task<Status> UpdateMerchantsAsync(SystemMerchantsDto merchant)
        {

            return await _systemMerchantRepo.UpdateMerchantsAsync(merchant);

        }
        public async Task<Status> DeleteMerchantAsync(int merchantId) {

            return await _systemMerchantRepo.DeleteMerchantAsync(merchantId);
        }
    }

}
