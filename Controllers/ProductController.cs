using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
using Minimart_Api.Services.ProductService;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("EditProducts")]
        public async Task<IActionResult> EditProducts([FromBody] AddProducts products)
        {
            try
            {
                var response = await _productService.EditProductsAsync(products);

                return Ok(response);


            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
