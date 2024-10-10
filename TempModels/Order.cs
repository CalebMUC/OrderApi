using System.ComponentModel.DataAnnotations.Schema;

namespace Minimart_Api.TempModels
{
    public class Order
    {
        public string? OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryScheduleDate { get; set; }
        public string? OrderedBy { get; set; }
        //public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public int Status { get; set; } 
        // This should map correctly to PaymentDetails
        [ForeignKey("PaymentDetails")]
        public int PaymentMethodID { get; set; }

        // Navigation property for PaymentDetails
        public PaymentDetails PaymentDetails { get; set; }

        public string PaymentConfirmation { get; set; } = string.Empty;
        public double TotalOrderAmount { get; set; }
        public double TotalPaymentAmount { get; set; }
        public double TotalDeliveryFees { get; set; }
        public double TotalTax { get; set; }

        public string? ShippingAddress { get; set; }
        public string ProductsJson { get; set; }
        public string PickupLocation { get; set; }
        public string PaymentDetailsJson { get; set; }

        public ICollection<OrderProducts> OrderProducts { get; set; }
    }


    //public class Product
    //{
    //    public string ProductID { get; set; }

    //    public string ProductName { get; set; }

    //    public int Quantity { get; set; }

    //    public double Price { get; set; }

    //    public double DeliveryFee { get; set; }
    //}

    //public class PaymentDetails
    //{
    //    public string PaymentMethodID { get; set; }

    //    // Store only relevant reference based on the method selected
    //    public string PaymentReference { get; set; }

    //    public double Amount { get; set; }
    //}

    //// Order status as enum for better readability
    //public enum OrderStatus
    //{
    //    Pending,
    //    Confirmed,
    //    Shipped,
    //    Delivered,
    //    Canceled
    //}
}
