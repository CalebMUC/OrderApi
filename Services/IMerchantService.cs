using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface IMerchantService
    {
        Task<IList<Merchants>> GetMerchantsAsync();

        Task<IList<BusinessTypes>> GetBusinessTypes();

        Task <ResponseStatus> AddMerchants(MerchantDTO merchantDTO);
    }
}
