using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
using Minimart_Api.Services;
using Minimart_Api.TempModels;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
       public CategoryController(ICategoryService categoryService) {

            _categoryService = categoryService;
        }

                // POST: api/SubcategoryFeature/AddFeatures
                [HttpPost("AddFeatures")]
                public async Task<IActionResult> AddFeaturesToSubcategory([FromBody] AddFeaturesDTO request)
                {
            try
            {
                if (request.Features == null || !request.Features.Any())
                {
                    return BadRequest("No features provided.");
                }

                var Response = await _categoryService.AddFeatures(request);
                return Ok(Response);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
                }
        [HttpPost("AddEditCategories")]
        public async Task<IActionResult> AddEditCategories(AddCategoryDTO addCategory)
        {
            try {

                var Response = await _categoryService.AddCategories(addCategory);

                return Ok(Response);
            }
            catch (Exception ex) {

                return BadRequest(ex.Message);
            }

        }

        // GET: api/SubcategoryFeature/GetFeatures/{subcategoryId}
        [HttpGet("GetSearchProducts/{subcategoryId}")]
        public async Task<IActionResult> GetSearchProducts(string subcategoryId)
        {
            var products= await _categoryService.GetSearchProducts(subcategoryId);

            if (products == null || !products.Any())
            {
                return NotFound("No features found for this subcategory.");
            }

            return Ok(products);
        }

        // POST
        [HttpPost("GetFilteredProducts")]
        public async Task<IActionResult> GetFilteredProducts(FilteredProductsDTO filteredProducts)
        {
            var products = await _categoryService.GetFilteredProducts(filteredProducts);




            if (products == null || !products.Any())
            {
                return NotFound("No features found for this subcategory.");
            }

            return Ok(products);
        }
      
        // GET: api/SubcategoryFeature/GetFeatures/{subcategoryId}
        [HttpPost("GetFeatures")]
                public async Task<IActionResult> GetFeaturesForSubcategory(FeatureRequestDTO feature)
                {
                    var features = await _categoryService.GetFeatures(feature);

                    if (features == null || !features.Any())
                    {
                //return NotFound("No features found for this subcategory.");

                return Ok(features);
                    }

                    return Ok(features);
                }
    }
}
