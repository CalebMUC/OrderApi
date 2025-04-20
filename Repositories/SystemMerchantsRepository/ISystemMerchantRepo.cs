using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.SystemMerchantsRepository
{
    public interface ISystemMerchantRepo
    {
        Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync();
        Task <SystemMerchants> GetMerchantByIdAsync(int merchantId);
        Task<Status> AddMerchantsAsync(SystemMerchantsDto merchant);
        Task<Status> UpdateMerchantsAsync(SystemMerchantsDto merchant);
        Task<Status> DeleteMerchantAsync(int merchantId);
    }
}
