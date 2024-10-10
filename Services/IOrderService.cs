using Minimart_Api.DTOS;

namespace Minimart_Api.Services
{
    public interface IOrderService
    {
        Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID);

    }
}
