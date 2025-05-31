using Minimart_Api.DTOS.Cart;
using Minimart_Api.Repositories.Search;
using Minimart_Api.Services.SearchService.SearchService;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Microsoft.Extensions.Caching.Memory;
using Minimart_Api.DTOS.Search;

namespace Minimart_Api.Services.SearchService
{
    public class SearchServices : ISearchService
    {
        private readonly ISearchRepo _searchRepo;
        private readonly ILogger<SearchServices> _logger;
        private readonly IMemoryCache _memoryCache;

        public SearchServices(ISearchRepo searchRepo, ILogger<SearchServices> logger, IMemoryCache memoryCache)
        {
            _searchRepo = searchRepo;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<string>> GetSearchSuggestion(string queryName, int limit = 10) {

            //check if the query is Null or Empty
            if (string.IsNullOrWhiteSpace(queryName))
                return Enumerable.Empty<string>();

            var cacheKey = $"search_suggestions_{queryName.ToLower()}";

            //Check if there was previously fetched suggestions
            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<string> suggestions)) {

                suggestions = await _searchRepo.GetSearchSuggestion(queryName, limit);

                //set cacheOptions //set SlidingExpirtion and AbsoluteExpiration
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(15))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                _memoryCache.Set(cacheKey, suggestions, cacheOptions);
            }

            return suggestions;

        }

        public async Task<IEnumerable<GetProductsDto>> SearchProductsAsync(string queryName) {

            return await _searchRepo.SearchProductsAsync(queryName);
        }
        public async Task<IEnumerable<Categories>> GetSearchResults(string queryname)
        {
            return await _searchRepo.GetSearchResults(queryname);
        }

        public async Task<Status> UpdateColumnJson()
        {
            return await _searchRepo.UpdateColumnJson();
        }

        public async Task<IEnumerable<CartResults>> GetSearchProducts(int CategoryID)
        {
            return await _searchRepo.GetSearchProducts(CategoryID);
        }


        public async Task<PaginatedResult<Products>> GetFilteredProducts(ProductFilterParams filterParams)
        {
            return await _searchRepo.GetFilteredProducts(filterParams);
        }

    }
}
