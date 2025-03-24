using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.DTOS.Orders;
using Minimart_Api.Services;
using Minimart_Api.Services.RabbitMQ;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        //private readonly IMyService _myService;
        private readonly IOrderService _orderService;
        private readonly IOrderEventPublisher _orderEventPublisher;


        public OrderController(IOrderService orderService, IOrderEventPublisher orderEventPublisher) { 

            _orderService = orderService;
            _orderEventPublisher = orderEventPublisher;

        }
        [HttpPost("GetOrders")]
        public async Task<IActionResult> GetOrders([FromBody] OrderRequest request)
        {
            var orders = await _orderService.GetOrdersByStatusAsync(request.Status,request.userID);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given status.");
            }
            return Ok(orders);
        }
        [HttpGet("GetOrderStatus")]
        public async Task<IActionResult> GetOrderStatus()
        {
            var orders = await _orderService.GetOrderStatusAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given status.");
            }
            return Ok(orders);
        }
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatusAsync(OrderTrackingDTO orderTracking)
        {
            try
            {
                var response = await _orderService.UpdateOrderStatusAsync(orderTracking);
                return Ok(response);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("GetOrderTracking")]
        public async Task<IActionResult> GetOrderTrackingAsync(GetOrderTrackingStatus trackingStatus)
        {
            try
            {
                var response = await _orderService.GetOrderTrackingAsync(trackingStatus);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetOrdersById")]
        public async Task<IActionResult> GetOrdersByIdAsync(string OrderId)
        {
            var orders = await _orderService.GetOrdersByIdAsync(OrderId);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given status.");
            }
            return Ok(orders);
        }

        [HttpPost("GetAdminOrders")]
        public async Task<IActionResult> GetAdminOrdersAsync()
        {
            var orders = await _orderService.GetAdminOrdersAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given status.");
            }
            return Ok(orders);
        }
        [HttpPost("GetMerchantOrders")]
        public async Task<IActionResult> GetMerchantOrdersAsync(MerchantRequestDto requestDto)
        {
            var orders = await _orderService.GetMerchantOrdersAsync(requestDto);
            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the given status.");
            }
            return Ok(orders);
        }

        [HttpPost("AddOrder")]
        public async Task<IActionResult> AddOrder(OrderListDto orderDTO)
        {

            if (orderDTO == null)
            {
                return BadRequest("No DATA");
            }
            try
            {
                var response = await _orderService.AddOrder(orderDTO);
                return Ok(response);

                //call react page here for printing by passing the response

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
