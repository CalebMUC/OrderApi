using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;

namespace Minimart_Api.Services.Cart
{
    public interface ICartService
    {
        Task<Status> AddToCart(string CartItems);
        Task<Status> DeleteCartItems(CartItemsDTO CartItems);
        Task<IEnumerable<CartResults>> GetCartItems(int UserID);

        Task<IEnumerable<CartResults>> GetBoughtItems(int userId);

        Task<SavedProductsDto> SaveItemAsync(SaveItemDto itemDto);
        Task<bool> RemoveItemAsync(int userId, string productId);
        Task<IEnumerable<SavedProductsDto>> GetSavedItemsAsync(int userId);
        //Task<Status> SaveItems(SaveItemsDTO saveItems);
        //Task<IEnumerable<Products>> GetSavedItems();

    }
}
