using Minimart_Api.DTOS.GuestCheckout;

namespace Minimart_Api.Services.GuestCheckout
{
    public interface IGuestCartService
    {
        Task<GuestCartResponseDto> AddToCartAsync(AddToGuestCartDto dto);
        Task<GuestCartResponseDto> GetCartAsync(string guestId);
        Task<bool> UpdateQuantityAsync(UpdateGuestCartDto dto, string guestId);
        Task<bool> RemoveItemAsync(int guestCartId, string guestId);
        Task<bool> ClearCartAsync(string guestId);
        Task<int> CleanupExpiredCartsAsync();
    }
}
