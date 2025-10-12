namespace Minimart_Api.DTOS.Payments
{
    public class MpesaTrxQueryRes
    {
        public bool success { get; set; }
        public string CheckoutRequestID { get; set; }
        public string MpesaReceiptNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string ResultCode { get; set; }
        public string ResultDesc { get; set; }
        public decimal Amount { get; set; }
    }
}
