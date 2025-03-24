using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.DTOS.Orders;
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

        public async Task<List<OrderStatus>> GetOrderStatusAsync()
        {
            return await _orderRepository.GetOrderStatusAsync();
        }

        public async Task<ResponseStatus> UpdateOrderStatusAsync(OrderTrackingDTO orderTracking)
        {
            return await _orderRepository.UpdateOrderStatusAsync(orderTracking);
        }
        public async Task<List<OrderTracking>> GetOrderTrackingAsync(GetOrderTrackingStatus trackingStatus) {

            return await _orderRepository.GetOrderTrackingAsync(trackingStatus);
        }
        public async Task<List<GetOrdersDTO>> GetOrdersByIdAsync(string OrderId)
        {
            return await _orderRepository.GetOrdersByIdAsync(OrderId);
        }

        public async Task<List<MerchantOrderDto>> GetAdminOrdersAsync() {
            return await _orderRepository.GetAdminOrdersAsync();
        }

        public async Task<List<MerchantOrderDto>> GetMerchantOrdersAsync(MerchantRequestDto requestDto)
        {
            return await _orderRepository.GetMerchantOrdersAsync(requestDto);
        }

        public async Task<ResponseStatus> AddOrder(OrderListDto orderDTO)
        {
            return await _orderRepository.AddOrder(orderDTO);
        }
    }
}
