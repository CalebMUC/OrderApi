using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories
{
    public interface IorderRepository
    {
        Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID); // This is the correct interface method declaration

        public Task<ResponseStatus> AddOrder(OrderListDto transaction);


    }
}
