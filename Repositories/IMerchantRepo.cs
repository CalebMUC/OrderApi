using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories
{
    public interface IMerchantRepo
    {
        Task<IList<Merchants>> GetMerchantsAsync();
        Task<IList<BusinessTypes>> GetBusinessTypes();
        Task<ResponseStatus> AddMerchants(MerchantDTO merchantDTO);
    }
}
