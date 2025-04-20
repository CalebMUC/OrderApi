using Microsoft.AspNetCore.Mvc;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Services.OpenSearchService;
using Minimart_Api.Services.SearchService.SearchService;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly IOpenSearchService _openSearchService;

        public SearchController(ISearchService searchService, IOpenSearchService openSearchService) { 
            _searchService = searchService;
            _openSearchService = openSearchService;
        }

        [HttpGet("ConvertToJson")]
        public async Task<IActionResult> ConvertToJson()
        {
            try
            {

                var Response = await _searchService.UpdateColumnJson();

                return Ok(Response);

            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        // POST: api/Search/CreateIndex
        [HttpPost("CreateIndex")]
        public async Task<IActionResult> CreateIndex([FromQuery] string indexName)
        {
            try
            {
                await _openSearchService.CreateIndexAsync(indexName);
                return Ok($"Index '{indexName}' created successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to create index: {ex.Message}");
            }
        }

        // GET: api/Search/SearchProducts
        [HttpGet("SearchProducts")]
        public async Task<IActionResult> SearchProducts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query is required.");
            }

            try
            {
                var products = await _openSearchService.SearchProductsAsync(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"Search operation failed: {ex.Message}");
            }
        }


        // GET: api/Search/Autocomplete
        [HttpGet("Autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query for autocomplete is required.");
            }

            try
            {
                var suggestions = await _openSearchService.AutocompleteAsync(query);
                return Ok(suggestions);
            }
            catch (Exception ex)
            {
                return BadRequest($"Autocomplete operation failed: {ex.Message}");
            }
        }



        [HttpGet("GetSearchResults")]
        public async Task<IActionResult> GetSearchResults([FromQuery] string queryname)
        {
            try { 

                var Response = await _searchService.GetSearchResults(queryname);

                return Ok(Response);

            }catch (Exception ex)
            {
                //throw new Exception(ex.Message);

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetSearchProducts/{subcategoryId}")]
        public async Task<IActionResult> GetSearchProducts(int categoryId)
        {
            var products = await _searchService.GetSearchProducts(categoryId);

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
            var products = await _searchService.GetFilteredProducts(filteredProducts);




            if (products == null || !products.Any())
            {
                return NotFound("No features found for this subcategory.");
            }

            return Ok(products);
        }

    }
}
