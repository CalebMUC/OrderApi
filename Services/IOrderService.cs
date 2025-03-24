using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.DTOS.Orders;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface IOrderService
    {
        Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID);

        Task<List<OrderStatus>> GetOrderStatusAsync();
        Task<ResponseStatus> UpdateOrderStatusAsync(OrderTrackingDTO orderTracking);
        Task<List<OrderTracking>> GetOrderTrackingAsync(GetOrderTrackingStatus trackingStatus);
        Task<List<GetOrdersDTO>> GetOrdersByIdAsync(string OrderId);
        Task<List<MerchantOrderDto>> GetMerchantOrdersAsync(MerchantRequestDto requestDto); // This is the correct interface method declaration
        Task<List<MerchantOrderDto>> GetAdminOrdersAsync();
        public Task<ResponseStatus> AddOrder(OrderListDto transaction);


    }
}
