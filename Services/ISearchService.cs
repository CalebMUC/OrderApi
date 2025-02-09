using Minimart_Api.TempModels;

namespace Minimart_Api.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<TSubcategoryid>> GetSearchResults(string queryname);

        Task<ResponseStatus> UpdateColumnJson();
    }
}
