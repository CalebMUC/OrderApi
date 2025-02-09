using Minimart_Api.DTOS;
using Minimart_Api.Repositories;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public class OrderService:IOrderService
    {
        private readonly IorderRepository _orderRepository;

        public OrderService(IorderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID)
        {
            return await _orderRepository.GetOrdersByStatusAsync(status,userID);
        }

        public async Task<ResponseStatus> AddOrder(OrderListDto orderDTO)
        {
            return await _orderRepository.AddOrder(orderDTO);
        }
    }
}
