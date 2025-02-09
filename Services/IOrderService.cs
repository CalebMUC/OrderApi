using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface IOrderService
    {
        Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID);
        public Task<ResponseStatus> AddOrder(OrderListDto transaction);

    }
}
