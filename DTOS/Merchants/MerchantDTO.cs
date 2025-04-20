namespace Minimart_Api.DTOS.Merchants
{
    public class MerchantDTO
    {
        public string MerchantName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BusinessType { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int County { get; set; }
        public int Town { get; set; }
        public string ExtraInformation { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phonenumber { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }
}
