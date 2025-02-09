using Minimart_Api.Repositories;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchRepo _searchRepo;

        public SearchService(ISearchRepo searchRepo) { 
            _searchRepo = searchRepo;
        }
        public async Task<IEnumerable<TSubcategoryid>> GetSearchResults(string queryname)
        {
            return await _searchRepo.GetSearchResults(queryname);
        }

        public async Task<ResponseStatus> UpdateColumnJson()
        {
            return await _searchRepo.UpdateColumnJson();
        }

    }
}
