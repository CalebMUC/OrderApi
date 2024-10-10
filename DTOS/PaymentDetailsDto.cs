namespace Minimart_Api.DTOS
{
    public class PaymentDetailsDto
    {
        
            public int PaymentID { get; set; }
            public string PaymentReference { get; set; }
            public double Amount { get; set; }
            public DateTime PaymentDate { get; set; }
        

    }
}
