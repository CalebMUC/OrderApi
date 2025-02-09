using System.Threading.Tasks;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;

namespace Minimart_Api.Services.OpenSearchService
{
    public interface IOpenSearchService
    {
        Task CreateIndexAsync(string indexname);
        Task IndexProductAsync(TProduct product);
        Task<IEnumerable<TProduct>> SearchProductsAsync(string query);
        Task<IEnumerable<string>> AutocompleteAsync(string query);
    }
}
