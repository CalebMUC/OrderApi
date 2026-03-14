namespace Minimart_Api.Utilities
{
    /// <summary>
    /// Utility for rounding prices to whole numbers for Kenya market (M-Pesa compatibility)
    /// </summary>
    public static class PriceUtility
    {
        /// <summary>
        /// Round price to nearest whole number (Away from zero for .5 cases)
        /// Example: 898.49 ? 898, 898.50 ? 899, 898.67 ? 899
        /// </summary>
        public static decimal RoundPrice(decimal price)
        {
            return Math.Round(price, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Round multiple prices
        /// </summary>
        public static List<decimal> RoundPrices(IEnumerable<decimal> prices)
        {
            return prices.Select(RoundPrice).ToList();
        }

        /// <summary>
        /// Calculate discounted price and round to whole number
        /// </summary>
        public static decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountPercentage)
        {
            if (discountPercentage <= 0)
                return RoundPrice(originalPrice);

            var discountedPrice = originalPrice * (1 - discountPercentage / 100m);
            return RoundPrice(discountedPrice);
        }

        /// <summary>
        /// Calculate total with delivery and round
        /// </summary>
        public static decimal CalculateTotal(decimal subtotal, decimal deliveryFee, decimal tax = 0)
        {
            return RoundPrice(subtotal + deliveryFee + tax);
        }

        /// <summary>
        /// Validate price is whole number
        /// </summary>
        public static bool IsWholeNumber(decimal price)
        {
            return price == Math.Floor(price);
        }

        /// <summary>
        /// Format price for display (KES format)
        /// </summary>
        public static string FormatKES(decimal price)
        {
            return $"KES {RoundPrice(price):N0}"; // N0 = no decimal places with thousand separators
        }

        /// <summary>
        /// Format price for display without currency
        /// </summary>
        public static string FormatAmount(decimal price)
        {
            return RoundPrice(price).ToString("N0"); // Example: 1,899
        }
    }
}
