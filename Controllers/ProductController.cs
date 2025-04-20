using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Category;
using Minimart_Api.DTOS.Products;
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


        [HttpPost("FetchAllProducts")]
        //public async Task<IActionResult> LoadMainImages()
        public async Task<IActionResult> FetchAllProducts()
        {


            try
            {
                var Response = await _productService.FetchAllProducts();

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("AddProducts")]
        public async Task<IActionResult> AddProducts([FromBody] AddProducts products)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _productService.AddProducts(products);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("LoadProductImages")]
        public async Task<IActionResult> LoadProductImages([FromBody] ProductDetails productDetails)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _productService.LoadProductImages(productDetails.ProductID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }


        [HttpPost("GetProductsByCategory")]
        public async Task<IActionResult> GetProductsByCategory([FromBody] SubCategory categoryName)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _productService.GetProductsByCategory(categoryName.CategoryID);
                 
                return Ok(Response);
            }
            catch (Exception ex)
            {
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
