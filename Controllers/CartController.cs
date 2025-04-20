using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Services.Cart;
using Newtonsoft.Json;

namespace Minimart_Api.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService) { 
            _cartService = cartService;
        }


        [HttpPost("AddCartItems")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCart cartitems)
        {
            var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.AddToCart(jsonSrting);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPost("DeleteCartItems")]
        public async Task<IActionResult> DeleteCartItems([FromBody] CartItemsDTO cartitems)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.DeleteCartItems(cartitems);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("SaveItems")]
        public async Task<IActionResult> SaveItems([FromBody] SaveItemsDTO saveItems)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.SaveItems(saveItems);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("GetCartItems")]
        public async Task<IActionResult> GetCartItems([FromBody] GetCartItems cartitems)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.GetCartItems(cartitems.UserID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpGet("GetSavedItems")]
        public async Task<IActionResult> GetSavedItems()
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.GetSavedItems();

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
    }
}
