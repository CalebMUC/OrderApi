using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Repositories
{
    public interface ISearchRepo
    {
        Task<IEnumerable<TSubcategoryid>> GetSearchResults(string queryname);

        Task<ResponseStatus> UpdateColumnJson();
    }
}
