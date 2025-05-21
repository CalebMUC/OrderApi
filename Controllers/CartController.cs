using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Services.Cart;
using Minimart_Api.Services.Recommedation;
using Minimart_Api.Services.SimilarProducts;
using Newtonsoft.Json;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ISimilarProductsService _similarProductsService;
        private readonly IRecomedationService _recomedationService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger,
            ISimilarProductsService similarProductsService,
            IRecomedationService recomedationService) { 
            _cartService = cartService;
            _similarProductsService = similarProductsService;
            _recomedationService = recomedationService;
            _logger = logger;
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

        //[HttpPost("SaveItems")]
        //public async Task<IActionResult> SaveItems([FromBody] SaveItemsDTO saveItems)
        //{
        //    //var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        var Response = await _cartService.SaveItems(saveItems);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}

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


        [HttpPost("GetBoughtItems")]
        public async Task<IActionResult> GetBoughtItems([FromBody] GetCartItems cartitems)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _cartService.GetBoughtItems(cartitems.UserID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("SaveItems")]
        public async Task<ActionResult<SavedProductsDto>> SaveItem([FromBody] SaveItemDto itemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _cartService.SaveItemAsync(itemDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving item for user {UserId}", itemDto.UserId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveItem(int userId, string productId)
        {
            try
            {
                var success = await _cartService.RemoveItemAsync(userId, productId);
                if (!success)
                {
                    return NotFound("Item not found");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing saved item for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<SavedProductsDto>>> GetSavedItems(int userId)
        {
            try
            {
                var savedItems = await _cartService.GetSavedItemsAsync(userId);
                return Ok(savedItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting saved items for user {UserId}", userId);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("personalized/{userId}")]
        public async Task<IActionResult> GetPersonalized(int userId, [FromQuery] int limit = 5)
        {
            var recommendations = await _recomedationService
                .GetPersonalizedRecommendations(userId, limit);
            return Ok(recommendations);
        }

        [HttpGet("complementary/{productId}")]
        public async Task<IActionResult> GetComplementary(string productId, [FromQuery] int limit = 5)
        {
            var recommendations = await _recomedationService
                .GetComplementaryProducts(productId, limit);
            return Ok(recommendations);
        }

        [HttpGet("frequently-bought/{productId}")]
        public async Task<IActionResult> GetFrequentlyBought(string productId, [FromQuery] int limit = 5)
        {
            var recommendations = await _recomedationService
                .GetFrequentlyBoughtTogether(productId, limit);
            return Ok(recommendations);
        }

        [HttpGet("GetSimilarProducts")]
        public async Task<ActionResult<IEnumerable<SimilarProductDto>>> GetSimilarProducts(
        string productId,
        [FromQuery] int limit = 5)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    return BadRequest("Product ID is required");
                }

                if (limit <= 0 || limit > 20)
                {
                    return BadRequest("Limit must be between 1 and 20");
                }

                var similarProducts = await _similarProductsService.GetSimilarProductsAsync(productId, limit);

                if (!similarProducts.Any())
                {
                    return Ok(new
                    {
                        message = "No similar products found with similarity score of 50 or above"
                    });
                }
                //
                return Ok(similarProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting similar products for {ProductId}", productId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

    }
}
