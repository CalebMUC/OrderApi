using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS.GuestCheckout;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.Models;
using Minimart_Api.Models.Enums;
using Minimart_Api.Utilities;
using Newtonsoft.Json;
using System.Text;

namespace Minimart_Api.Services.GuestCheckout
{
    public class GuestCheckoutService : IGuestCheckoutService
    {
        private readonly MinimartDBContext _context;
        private readonly ILogger<GuestCheckoutService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGuestCartService _guestCartService;

        public GuestCheckoutService(
            MinimartDBContext context,
            ILogger<GuestCheckoutService> logger,
            IConfiguration configuration,
            IGuestCartService guestCartService)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
            _guestCartService = guestCartService;
        }

        public async Task<GuestCheckoutResponseDto> InitiateCheckoutAsync(InitiateGuestCheckoutDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Get guest cart
                var cart = await _guestCartService.GetCartAsync(dto.GuestId);

                if (cart.Items == null || !cart.Items.Any())
                {
                    throw new ArgumentException("Cart is empty. Please add items before checking out.");
                }

                // 2. Validate all items are still available
                foreach (var item in cart.Items)
                {
                    if (!item.IsActive)
                    {
                        throw new ArgumentException($"Product '{item.ProductName}' is no longer available.");
                    }

                    if (item.StockQuantity < item.Quantity)
                    {
                        throw new ArgumentException($"Insufficient stock for '{item.ProductName}'. Available: {item.StockQuantity}");
                    }
                }

                // 3. Format phone number
                var formattedPhone = PhoneNumberUtility.FormatForStorage(dto.PhoneNumber);
                var mpesaPhone = PhoneNumberUtility.FormatForMpesa(dto.PhoneNumber);

                // 4. Calculate delivery fee
                var deliveryFee = await CalculateDeliveryFeeAsync(dto.CountyId, dto.TownId, dto.DeliveryStationId);

                // 5. Calculate totals and ROUND TO WHOLE NUMBERS
                var subtotal = PriceUtility.RoundPrice(cart.Subtotal);
                var roundedDeliveryFee = PriceUtility.RoundPrice(deliveryFee);
                var total = PriceUtility.RoundPrice(subtotal + roundedDeliveryFee);
                
                _logger.LogInformation(
                    "Guest checkout amounts - Subtotal: {Subtotal}, Delivery: {Delivery}, Total: {Total}",
                    subtotal, roundedDeliveryFee, total);

                // 6. Create GuestCheckout record
                var guestCheckout = new Models.GuestCheckout
                {
                    GuestCheckoutId = Guid.NewGuid(),
                    GuestId = dto.GuestId,
                    FullName = dto.FullName,
                    PhoneNumber = formattedPhone,
                    Email = dto.Email,
                    DeliveryAddress = dto.DeliveryAddress,
                    CountyId = dto.CountyId,
                    TownId = dto.TownId,
                    DeliveryStationId = dto.DeliveryStationId,
                    DeliveryFee = roundedDeliveryFee,
                    SubtotalAmount = subtotal,
                    TotalAmount = total,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.GuestCheckouts.Add(guestCheckout);
                await _context.SaveChangesAsync();

                // 7. Initiate M-Pesa STK Push
                var stkPushResult = await InitiateMpesaSTKPushAsync(
                    mpesaPhone,
                    total,
                    guestCheckout.GuestCheckoutId.ToString()
                );

                // 8. Update checkout with M-Pesa details
                guestCheckout.CheckoutRequestId = stkPushResult.CheckoutRequestID;
                guestCheckout.MerchantRequestId = stkPushResult.MerchantRequestID;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Guest checkout initiated: {GuestCheckoutId} for guest {GuestId}, Total: {Total}, CheckoutRequestId: {CheckoutRequestId}",
                    guestCheckout.GuestCheckoutId, dto.GuestId, total, stkPushResult.CheckoutRequestID);

                return new GuestCheckoutResponseDto
                {
                    GuestCheckoutId = guestCheckout.GuestCheckoutId,
                    Status = "Pending",
                    SubtotalAmount = subtotal,
                    DeliveryFee = roundedDeliveryFee,
                    TotalAmount = total,
                    CheckoutRequestId = stkPushResult.CheckoutRequestID,
                    MerchantRequestId = stkPushResult.MerchantRequestID,
                    Message = stkPushResult.CustomerMessage ?? "Please check your phone to complete payment"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error initiating guest checkout for guest {GuestId}", dto.GuestId);
                throw;
            }
        }

        public async Task<GuestCheckoutStatusDto> GetCheckoutStatusAsync(Guid guestCheckoutId)
        {
            try
            {
                var checkout = await _context.GuestCheckouts
                    .FirstOrDefaultAsync(gc => gc.GuestCheckoutId == guestCheckoutId);

                if (checkout == null)
                {
                    throw new ArgumentException("Checkout not found");
                }

                return new GuestCheckoutStatusDto
                {
                    GuestCheckoutId = checkout.GuestCheckoutId,
                    Status = checkout.Status,
                    OrderId = checkout.OrderId,
                    MpesaReceiptNumber = checkout.MpesaReceiptNumber,
                    CreatedAt = checkout.CreatedAt,
                    CompletedAt = checkout.CompletedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting checkout status for {GuestCheckoutId}", guestCheckoutId);
                throw;
            }
        }

        public async Task<bool> ProcessPaymentConfirmationAsync(string checkoutRequestId, string mpesaReceiptNumber)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Find the guest checkout
                var guestCheckout = await _context.GuestCheckouts
                    .Include(gc => gc.County)
                    .Include(gc => gc.Town)
                    .Include(gc => gc.DeliveryStation)
                    .FirstOrDefaultAsync(gc => gc.CheckoutRequestId == checkoutRequestId && gc.Status == "Pending");

                if (guestCheckout == null)
                {
                    _logger.LogWarning("Guest checkout not found for CheckoutRequestId: {CheckoutRequestId}", checkoutRequestId);
                    return false;
                }

                // 2. Get guest cart items
                var cartItems = await _context.GuestCarts
                    .Include(gc => gc.Product)
                        .ThenInclude(p => p.Merchant)
                    .Where(gc => gc.GuestId == guestCheckout.GuestId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    throw new InvalidOperationException("Cart is empty. Cannot create order.");
                }

                // 3. Create Order ID
                var orderId = $"GC-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

                // 4. Create PaymentDetails
                var paymentDetails = new PaymentDetails
                {
                    PaymentID = Guid.NewGuid(),
                    PaymentMethodID = 1, // Assuming M-Pesa PaymentMethodID = 1
                    TrxReference = mpesaReceiptNumber,
                    PaymentReference = mpesaReceiptNumber,
                    Phonenumber = guestCheckout.PhoneNumber,
                    Amount = guestCheckout.TotalAmount,
                    Status = "Completed",
                    PaymentDate = DateTime.UtcNow
                };

                _context.PaymentDetails.Add(paymentDetails);

                // 5. Prepare products JSON
                var productsJson = cartItems.Select(ci => new
                {
                    ProductId = ci.Product.ProductId,
                    ProductName = ci.Product.ProductName,
                    Price = ci.Product.Price,
                    Discount = ci.Product.Discount,
                    Quantity = ci.Quantity,
                    MerchantId = ci.Product.MerchantID
                }).ToList();

                // 6. Prepare payment details JSON
                var paymentDetailsJson = new
                {
                    PaymentMethod = "M-Pesa",
                    MpesaReceiptNumber = mpesaReceiptNumber,
                    CheckoutRequestId = checkoutRequestId,
                    PhoneNumber = guestCheckout.PhoneNumber,
                    Amount = guestCheckout.TotalAmount,
                    TransactionDate = DateTime.UtcNow
                };

                // 7. Create Order
                var order = new Order
                {
                    OrderID = orderId,
                    ApplicationUserId = null, // Guest order
                    IsGuestOrder = true,
                    GuestCheckoutId = guestCheckout.GuestCheckoutId,
                    StatusID = 1, // Assuming 1 = Pending
                    Status = "Pending",
                    StatusEnum = OrderStatusEnum.Pending,
                    OrderDate = DateTime.UtcNow,
                    DeliveryScheduleDate = DateTime.UtcNow.AddDays(3), // Default 3 days
                    OrderedBy = guestCheckout.FullName,
                    PaymentID = paymentDetails.PaymentID,
                    PaymentConfirmation = mpesaReceiptNumber,
                    TotalOrderAmount = guestCheckout.SubtotalAmount,
                    TotalPaymentAmount = guestCheckout.TotalAmount,
                    TotalDeliveryFees = guestCheckout.DeliveryFee,
                    TotalTax = 0,
                    ShippingAddress = guestCheckout.DeliveryAddress,
                    PickupLocation = guestCheckout.DeliveryStation?.DeliveryStationName ?? "Home Delivery",
                    ProductsJson = JsonConvert.SerializeObject(productsJson),
                    PaymentDetailsJson = JsonConvert.SerializeObject(paymentDetailsJson)
                };

                _context.Orders.Add(order);

                // 8. Create OrderProducts
                foreach (var cartItem in cartItems)
                {
                    // ? Calculate discounted price using utility
                    var discountedPrice = PriceUtility.CalculateDiscountedPrice(
                        cartItem.Product.Price,
                        cartItem.Product.Discount
                    );
                    
                    var itemTotal = PriceUtility.RoundPrice(discountedPrice * cartItem.Quantity);

                    var orderProduct = new OrderProduct
                    {
                        OrderID = orderId,
                        ProductId = cartItem.Product.ProductId,
                        MerchantID = cartItem.Product.MerchantID,
                        Quantity = cartItem.Quantity,
                        TotalPrice = itemTotal,
                        Status = OrderStatusEnum.Pending,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };

                    _context.OrderProducts.Add(orderProduct);

                    // 9. Deduct stock
                    cartItem.Product.StockQuantity -= cartItem.Quantity;
                    cartItem.Product.UpdatedOn = DateTime.UtcNow;
                }

                // 10. Update guest checkout
                guestCheckout.Status = "Completed";
                guestCheckout.MpesaReceiptNumber = mpesaReceiptNumber;
                guestCheckout.OrderId = orderId;
                guestCheckout.CompletedAt = DateTime.UtcNow;

                // 11. Clear guest cart
                _context.GuestCarts.RemoveRange(cartItems);

                // 12. Save all changes
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation(
                    "Guest checkout {GuestCheckoutId} completed successfully. Order {OrderId} created with M-Pesa receipt {MpesaReceiptNumber}",
                    guestCheckout.GuestCheckoutId, orderId, mpesaReceiptNumber);

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error processing payment confirmation for CheckoutRequestId: {CheckoutRequestId}", checkoutRequestId);
                throw;
            }
        }

        public async Task<int> AbandonOldPendingCheckoutsAsync()
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-7);

                var oldCheckouts = await _context.GuestCheckouts
                    .Where(gc => gc.Status == "Pending" && gc.CreatedAt < cutoffDate)
                    .ToListAsync();

                if (!oldCheckouts.Any())
                {
                    return 0;
                }

                foreach (var checkout in oldCheckouts)
                {
                    checkout.Status = "Abandoned";
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Abandoned {Count} old pending guest checkouts", oldCheckouts.Count);

                return oldCheckouts.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error abandoning old pending checkouts");
                throw;
            }
        }

        public async Task<decimal> CalculateDeliveryFeeAsync(int? countyId, int? townId, int? deliveryStationId)
        {
            try
            {
                // Default delivery fee
                decimal deliveryFee = 200m;

                // If delivery station is specified, use its fee
                if (deliveryStationId.HasValue)
                {
                    var station = await _context.DeliveryStations
                        .FirstOrDefaultAsync(ds => ds.DeliveryStationId == deliveryStationId.Value);

                    // Note: DeliveryPrice might not exist in your model, adjust if needed
                    // if (station != null && station.DeliveryPrice.HasValue)
                    // {
                    //     deliveryFee = station.DeliveryPrice.Value;
                    // }
                }
                // Otherwise, calculate based on county/town
                else if (countyId.HasValue)
                {
                    var county = await _context.Counties
                        .FirstOrDefaultAsync(c => c.CountyId == countyId.Value);

                    if (county != null)
                    {
                        // Example logic: Counties outside Nairobi have higher fees
                        if (county.CountyName?.ToLower().Contains("nairobi") == true)
                        {
                            deliveryFee = 150m;
                        }
                        else
                        {
                            deliveryFee = 350m;
                        }
                    }
                }

                return deliveryFee;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating delivery fee");
                return 200m; // Return default on error
            }
        }

        #region M-Pesa Integration

        private async Task<STKPushResponse> InitiateMpesaSTKPushAsync(string phoneNumber, decimal amount, string accountReference)
        {
            try
            {
                // ? AMOUNT ALREADY ROUNDED - No need to round again
                // Amount is rounded when creating GuestCheckout record
                
                _logger.LogInformation(
                    "M-Pesa STK Push for phone {PhoneNumber}, Amount: {Amount} (whole number)",
                    phoneNumber, amount);

                var businessShortCode = _configuration["Mpesa:BusinessShortCode"] ?? "174379";
                var passkey = _configuration["Mpesa:Passkey"] ?? "bfb279f9aa9bdbcf158e97dd71a467cd2f54f2a74b1cfcfc9e68d8f7cbe72956";
                var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                var password = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{businessShortCode}{passkey}{timestamp}"));

                var callbackUrl = _configuration["Mpesa:CallbackUrl"] ?? "https://api.quickcrate.co.ke/api/mpesa/confirmation";

                var stkPushRequest = new STKPushRequest
                {
                    BusinessShortCode = businessShortCode,
                    Password = password,
                    Timestamp = timestamp,
                    TransactionType = "CustomerPayBillOnline",
                    Amount = amount, // Already whole number
                    PartyA = phoneNumber,
                    PartyB = businessShortCode,
                    PhoneNumber = phoneNumber,
                    CallBackURL = callbackUrl,
                    AccountReference = accountReference,
                    TransactionDesc = $"Payment for order {accountReference}"
                };

                // Get OAuth token
                var token = await GetMpesaAccessTokenAsync();

                // Send STK Push request
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var stkPushUrl = _configuration["Mpesa:STKPushUrl"] ?? "https://sandbox.safaricom.co.ke/mpesa/stkpush/v1/processrequest";
                var response = await client.PostAsJsonAsync(stkPushUrl, stkPushRequest);

                var responseContent = await response.Content.ReadAsStringAsync();
                var stkPushResponse = JsonConvert.DeserializeObject<STKPushResponse>(responseContent);

                if (stkPushResponse == null || stkPushResponse.ResponseCode != "0")
                {
                    throw new Exception($"STK Push failed: {stkPushResponse?.ResponseDescription ?? "Unknown error"}");
                }

                return stkPushResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating M-Pesa STK Push for phone {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        private async Task<string> GetMpesaAccessTokenAsync()
        {
            try
            {
                var consumerKey = _configuration["Mpesa:ConsumerKey"];
                var consumerSecret = _configuration["Mpesa:ConsumerSecret"];
                var authUrl = _configuration["Mpesa:AuthUrl"] ?? "https://sandbox.safaricom.co.ke/oauth/v1/generate?grant_type=client_credentials";

                using var client = new HttpClient();
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);

                var response = await client.GetAsync(authUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<dynamic>(content);

                return tokenResponse?.access_token?.ToString() ?? throw new Exception("Failed to get access token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting M-Pesa access token");
                throw;
            }
        }

        #endregion
    }
}
