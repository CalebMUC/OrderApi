using Minimart_Api.DTOS;
using Minimart_Api.Repositories;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public class MerchantService:IMerchantService
    {
        private readonly IMerchantRepo _merchantRepo;
        public MerchantService(IMerchantRepo merchantRepo) { 
            _merchantRepo = merchantRepo;
        }
        public async Task<IList<Merchants>> GetMerchantsAsync() { 

            return await _merchantRepo.GetMerchantsAsync();
        }
        public async Task<IList<BusinessTypes>> GetBusinessTypes()
        {

            return await _merchantRepo.GetBusinessTypes();
        }

        public async Task<ResponseStatus> AddMerchants(MerchantDTO merchantDTO)
        {

            return await _merchantRepo.AddMerchants(merchantDTO);
        }
    }
}
