using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories.SystemMerchantsRepository
{
    public interface ISystemMerchantRepo
    {
        Task<IEnumerable<SystemMerchants>> GetAllMerchantsAsync();
        Task <SystemMerchants> GetMerchantByIdAsync(int merchantId);
        Task<ResponseStatus> AddMerchantsAsync(SystemMerchantsDto merchant);
        Task<ResponseStatus> UpdateMerchantsAsync(SystemMerchantsDto merchant);
        Task<ResponseStatus> DeleteMerchantAsync(int merchantId);
    }
}
