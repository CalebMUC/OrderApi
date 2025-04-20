using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Orders;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.Models;
using System.ComponentModel.DataAnnotations;

public class OrderDTO
{
    public string? OrderID { get; set; }

    //public int? MerchantId { get; set; }

    public int UserID { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime DeliveryScheduleDate { get; set; }
    public string? OrderedBy { get; set; }
    public int Status { get; set; }
    public List<PaymentDetailsDto> PaymentDetails { get; set; }
    public List<OrderProductsDTO> Products { get; set; }
    public string PaymentConfirmation { get; set; }
    public double TotalOrderAmount { get; set; }
    public double TotalPaymentAmount { get; set; }
    public double TotalDeliveryFees { get; set; }
    public double TotalTax { get; set; }
    public ShippingAddress ShippingAddress { get; set; }
    public PickUpLocation PickUpLocation { get; set; }
}

public class OrderListDto
{
    [Required]
    public IList<OrderDTO> Orders { get; set; } = new List<OrderDTO>();
}
