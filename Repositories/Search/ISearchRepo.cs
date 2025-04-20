using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories.Search
{
    public interface ISearchRepo
    {
        Task<IEnumerable<Categories>> GetSearchResults(string queryname);

        Task<Status> UpdateColumnJson();
        Task<IEnumerable<CartResults>> GetSearchProducts(int CategoryID);

        Task<List<CartResults>> GetFilteredProducts(FilteredProductsDTO filteredProducts);


    }
}
