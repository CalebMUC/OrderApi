using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.Models;

namespace Minimart_Api.Services.SystemMerchantService
{
    public interface ISystemMerchants
    {
        Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync();
        Task<SystemMerchants> GetMerchantByIdAsync(int  merchantId);
        Task<Status> AddMerchantsAsync(SystemMerchantsDto merchant);

        Task<Status> UpdateMerchantsAsync(SystemMerchantsDto merchant);

        Task<Status> DeleteMerchantAsync(int merchantId);

    }
}
