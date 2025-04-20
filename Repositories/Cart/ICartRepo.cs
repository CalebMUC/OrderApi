using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;

namespace Minimart_Api.Repositories.Cart
{
    public interface ICartRepo
    {
        Task<IEnumerable<CartResults>> GetCartItems(int UserID);
        Task<Status> AddToCart(string CartItems); 

        Task<Status> DeleteCartItems(CartItemsDTO CartItems);
        Task<IEnumerable<Products>> GetSavedItems();
        Task<Status> SaveItems(SaveItemsDTO saveItems);
    }
}
