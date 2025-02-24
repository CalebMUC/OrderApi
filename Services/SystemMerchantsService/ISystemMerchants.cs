using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services.SystemMerchantService
{
    public interface ISystemMerchants
    {
        Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync();
        Task<SystemMerchants> GetMerchantByIdAsync(int  merchantId);
        Task<ResponseStatus> AddMerchantsAsync(SystemMerchantsDto merchant);

        Task<ResponseStatus> UpdateMerchantsAsync(SystemMerchantsDto merchant);

        Task<ResponseStatus> DeleteMerchantAsync(int merchantId);

    }
}
