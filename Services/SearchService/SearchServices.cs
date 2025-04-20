using Minimart_Api.DTOS.Cart;
using Minimart_Api.Repositories.Search;
using Minimart_Api.Services.SearchService.SearchService;
using Minimart_Api.Models;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;

namespace Minimart_Api.Services.SearchService
{
    public class SearchServices : ISearchService
    {
        private readonly ISearchRepo _searchRepo;

        public SearchServices(ISearchRepo searchRepo)
        {
            _searchRepo = searchRepo;
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


        public async Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts)
        {
            return await _searchRepo.GetFilteredProducts(filteredProducts);
        }

    }
}
