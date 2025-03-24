using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Category;
using Minimart_Api.Services;
using Minimart_Api.Services.CategoriesService;
using Minimart_Api.TempModels;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ICategoriesService _categoriesService;
       public CategoryController(ICategoryService categoryService, ICategoriesService categoriesService) {

            _categoryService = categoryService;
            _categoriesService = categoriesService;
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
        //[HttpPost("AddEditCategories")]
        //public async Task<IActionResult> AddEditCategories(AddCategoryDTO addCategory)
        //{
        //    try {

        //        var Response = await _categoryService.AddCategories(addCategory);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex) {

        //        return BadRequest(ex.Message);
        //    }

        //}

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _categoriesService.GetAllCategoriesAsync();
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetNestedCategories")]
        public async Task<IActionResult> GetNestedCategoriesAsync()
        {
            try
            {
                var response = await _categoriesService.GetNestedCategoriesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCategoriesById")]
        public async Task<IActionResult> GetCategoriesByIdAsync(int CategoryId)
        {
            try
            {
                var response = await _categoriesService.GetCategoryByIdAsync(CategoryId);
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("AddCategories")]
        public async Task<IActionResult> AddCategoriesAsync(CategoriesDto categories)
        {
            try
            {
                var response = await _categoriesService.AddCategoriesAsync(categories);
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("UpdateCategories")]
        public async Task<IActionResult> UpdateCategoriesAsync(CategoriesDto categories)
        {
            try
            {
                var response = await _categoriesService.UpdateCategoriesAsync(categories);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("DeleteCategoriesById")]
        public async Task<IActionResult> DeleteCategoriesByIdAsync(int CategoryId)
        {
            try
            {
                var response = await _categoriesService.DeleteCategoryAsync(CategoryId);
                return Ok(response);
            }
            catch (Exception ex)
            {
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
        [HttpGet("GetAllFeatures")]
        public async Task<IActionResult> GetAllFeatures()
        {
            try
            {
                var response = await _categoryService.GetAllFeatures();
                return Ok(response);
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
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

        //[HttpPost("SearchFilters")]
        //public async Task<IActionResult> SearchFilters(FeatureSearchQuery searchQuery)
        //{
        //    var features = await _categoryService.FeatureSearchFilter;

        //    if (features == null || !features.Any())
        //    {
        //        //return NotFound("No features found for this subcategory.");

        //        return Ok(features);
        //    }

        //    return Ok(features);
        //}
    }
}
