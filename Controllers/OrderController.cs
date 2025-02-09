using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
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
