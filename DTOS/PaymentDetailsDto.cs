namespace Minimart_Api.DTOS
{
    public class PaymentDetailsDto
    {
        
            public int PaymentID { get; set; }
            public long PaymentReference { get; set; }

            public string PaymentMethod { get; set; }

            public long Phonenumber { get; set; }
            public decimal Amount { get; set; }
            public DateTime PaymentDate { get; set; }
        

    }
}
