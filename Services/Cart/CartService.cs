using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Repositories.Cart;
using Minimart_Api.Models;

namespace Minimart_Api.Services.Cart
{
    public class CartService : ICartService
    {
        private readonly ICartRepo _cartRepo;
        public CartService(ICartRepo cartRepo ) { 
            _cartRepo = cartRepo;
        }


        public async Task<Status> AddToCart(string CartItems)
        {
            return await _cartRepo.AddToCart(CartItems);
        }


        public async Task<Status> DeleteCartItems(CartItemsDTO CartItems)
        {
            return await _cartRepo.DeleteCartItems(CartItems);
        }

        public async Task<Status> SaveItems(SaveItemsDTO saveItems)
        {
            return await _cartRepo.SaveItems(saveItems);
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int UserID)
        {
            return await _cartRepo.GetCartItems(UserID);
        }

        public async Task<IEnumerable<Products>> GetSavedItems()
        {
            return await _cartRepo.GetSavedItems();
        }


    }
}
