using System.ComponentModel.DataAnnotations;

namespace Minimart_Api.DTOS.GuestCheckout
{
    // ==========================================
    // GUEST CART DTOs
    // ==========================================
    
    public class AddToGuestCartDto
    {
        [Required(ErrorMessage = "Guest ID is required")]
        [MaxLength(100)]
        public string GuestId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product ID is required")]
        public Guid ProductId { get; set; }

        [Required]
        [Range(1, 9999, ErrorMessage = "Quantity must be between 1 and 9999")]
        public int Quantity { get; set; }
    }

    public class UpdateGuestCartDto
    {
        [Required]
        public int GuestCartId { get; set; }

        [Required]
        [Range(1, 9999, ErrorMessage = "Quantity must be between 1 and 9999")]
        public int Quantity { get; set; }
    }

    public class GuestCartItemDto
    {
        public int GuestCartId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Quantity { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }
        public decimal ItemTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class GuestCartResponseDto
    {
        public string GuestId { get; set; } = string.Empty;
        public List<GuestCartItemDto> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    // ==========================================
    // GUEST CHECKOUT DTOs
    // ==========================================
    
    public class InitiateGuestCheckoutDto
    {
        [Required(ErrorMessage = "Guest ID is required")]
        [MaxLength(100)]
        public string GuestId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^(0[17]\d{8}|254[17]\d{8})$", 
            ErrorMessage = "Invalid Kenyan phone number. Use format: 07XXXXXXXX, 01XXXXXXXX, or 2547XXXXXXXX")]
        public string PhoneNumber { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(255)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Delivery address is required")]
        [MinLength(10, ErrorMessage = "Delivery address must be at least 10 characters")]
        public string DeliveryAddress { get; set; } = string.Empty;

        public int? CountyId { get; set; }

        public int? TownId { get; set; }

        public int? DeliveryStationId { get; set; }
    }

    public class GuestCheckoutResponseDto
    {
        public Guid GuestCheckoutId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal SubtotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CheckoutRequestId { get; set; }
        public string? MerchantRequestId { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class GuestCheckoutStatusDto
    {
        public Guid GuestCheckoutId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? OrderId { get; set; }
        public string? MpesaReceiptNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }

    // ==========================================
    // PHONE UTILITIES
    // ==========================================
    
    public static class PhoneNumberUtility
    {
        /// <summary>
        /// Format Kenyan phone number for internal storage (07XXXXXXXX or 01XXXXXXXX)
        /// </summary>
        public static string FormatForStorage(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty");

            // Remove spaces, dashes, and plus signs
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("+", "");

            // Handle 2547XXXXXXXX or 2541XXXXXXXX format
            if (phoneNumber.StartsWith("254") && phoneNumber.Length == 12)
            {
                return "0" + phoneNumber.Substring(3);
            }

            // Handle 07XXXXXXXX or 01XXXXXXXX format
            if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
            {
                return phoneNumber;
            }

            // Handle 7XXXXXXXX or 1XXXXXXXX format
            if ((phoneNumber.StartsWith("7") || phoneNumber.StartsWith("1")) && phoneNumber.Length == 9)
            {
                return "0" + phoneNumber;
            }

            throw new ArgumentException($"Invalid Kenyan phone number format: {phoneNumber}");
        }

        /// <summary>
        /// Format Kenyan phone number for M-Pesa (2547XXXXXXXX or 2541XXXXXXXX)
        /// </summary>
        public static string FormatForMpesa(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty");

            // Remove spaces, dashes, and plus signs
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "").Replace("+", "");

            // Handle 2547XXXXXXXX or 2541XXXXXXXX format (already correct)
            if (phoneNumber.StartsWith("254") && phoneNumber.Length == 12)
            {
                return phoneNumber;
            }

            // Handle 07XXXXXXXX or 01XXXXXXXX format
            if (phoneNumber.StartsWith("0") && phoneNumber.Length == 10)
            {
                return "254" + phoneNumber.Substring(1);
            }

            // Handle 7XXXXXXXX or 1XXXXXXXX format
            if ((phoneNumber.StartsWith("7") || phoneNumber.StartsWith("1")) && phoneNumber.Length == 9)
            {
                return "254" + phoneNumber;
            }

            throw new ArgumentException($"Invalid Kenyan phone number format: {phoneNumber}");
        }

        /// <summary>
        /// Validate Kenyan phone number
        /// </summary>
        public static bool IsValid(string phoneNumber)
        {
            try
            {
                FormatForStorage(phoneNumber);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
