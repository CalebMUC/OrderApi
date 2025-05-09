using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Category;
using Minimart_Api.DTOS.Features;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Services;
using Minimart_Api.Services.CategoriesService;
using Minimart_Api.Models;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoriesService _categoriesService;
       public CategoryController(ICategoriesService categoriesService) {

            _categoriesService = categoriesService;
        }

                // POST: api/SubcategoryFeature/AddFeatures
            
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

        [HttpPost("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory([FromBody] SubCategory categoryName)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _categoriesService.GetSubCategory(categoryName.CategoryID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }




        // GET: api/SubcategoryFeature/GetFeatures/{subcategoryId}
        

  
      

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
