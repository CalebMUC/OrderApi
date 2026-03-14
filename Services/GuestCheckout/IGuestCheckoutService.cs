using Minimart_Api.DTOS.GuestCheckout;

namespace Minimart_Api.Services.GuestCheckout
{
    public interface IGuestCheckoutService
    {
        Task<GuestCheckoutResponseDto> InitiateCheckoutAsync(InitiateGuestCheckoutDto dto);
        Task<GuestCheckoutStatusDto> GetCheckoutStatusAsync(Guid guestCheckoutId);
        Task<bool> ProcessPaymentConfirmationAsync(string checkoutRequestId, string mpesaReceiptNumber);
        Task<int> AbandonOldPendingCheckoutsAsync();
        Task<decimal> CalculateDeliveryFeeAsync(int? countyId, int? townId, int? deliveryStationId);
    }
}
