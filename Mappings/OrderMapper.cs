using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Orders;
using Minimart_Api.Models;
using Newtonsoft.Json;

namespace Minimart_Api.Mappings
{
    public class OrderMapper
    {

        public static class ReportConfiguration
        {
            //map report types to thier actual Parameters
            public static readonly Dictionary<string, List<string>> ReportParameterMappings = new Dictionary<string, List<string>> {
            {"Sales Report", new List<string>{"FromDate","ToDate" } },
            {"Product Report", new List<string>{"fromDate","toDate","category" } },
            {"Customer Report", new List<string>{"fromDate","toDate", "customerID" } }
        };

        }
        // Mapping Order to OrderDTO
        public OrderDTO MapToDto(Orders order)
        {
            return new OrderDTO
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                OrderDate = DateTime.Now,
                OrderedBy = order.OrderedBy,
                Status = order.Status,


                PaymentConfirmation = order.PaymentConfirmation,
                TotalOrderAmount = order.TotalOrderAmount,
                TotalPaymentAmount = order.TotalPaymentAmount,
                TotalDeliveryFees = order.TotalDeliveryFees,
                TotalTax = order.TotalTax,
                ShippingAddress = JsonConvert.DeserializeObject<ShippingAddress>(order.ShippingAddress),
                Products = JsonConvert.DeserializeObject<List<OrderProductsDTO>>(order.ProductsJson),

                PickUpLocation = JsonConvert.DeserializeObject<PickUpLocation>(order.PickupLocation),
                //PaymentDetails = JsonConvert.DeserializeObject<PaymentDetailsDto>(order.PaymentDetailsJson)
            };
        }

        // Mapping OrderDTO back to Order
        public Orders MapToEntity(OrderDTO orderDto)
        {
            return new Orders
            {
                OrderID = orderDto.OrderID,
                UserID = orderDto.UserID,
                OrderDate = orderDto.OrderDate,
                OrderedBy = orderDto.OrderedBy,
                Status = orderDto.Status,

                // Map PaymentDetails JSON
                PaymentDetailsJson = JsonConvert.SerializeObject(new PaymentDetails
                {
                    PaymentID = orderDto.PaymentDetails.First().PaymentID, // Access the first item's PaymentID
                   // TrxReference = orderDto.PaymentDetails.First().PaymentReference,
                    Amount = orderDto.PaymentDetails.First().Amount,
                    PaymentDate = DateTime.UtcNow // Use the current date
                }),


            // Map Products JSON
            ProductsJson = JsonConvert.SerializeObject(orderDto.Products.Select(p => new
                {
                    ProductName = p.ProductName,
                    ProductID = p.ProductID,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    Discount = p.Discount,
                }).ToList()),

                // Map the collection of OrderProducts
                OrderProducts = orderDto.Products.Select(productDto => new OrderProducts
                {
                    ProductID = productDto.ProductID,
                    Quantity = productDto.Quantity,
                }).ToList(),

                // Map Payment Confirmation
                PaymentConfirmation = orderDto.PaymentConfirmation,
                TotalOrderAmount = orderDto.TotalOrderAmount,
                TotalPaymentAmount = orderDto.TotalPaymentAmount,
                TotalDeliveryFees = orderDto.TotalDeliveryFees,
                TotalTax = orderDto.TotalTax,

                // Map ShippingAddress JSON
                ShippingAddress = JsonConvert.SerializeObject(new ShippingAddress
                {
                    Address = orderDto.ShippingAddress.Address,
                    County = orderDto.ShippingAddress.County,
                    Town = orderDto.ShippingAddress.Town,
                    PostalCode = orderDto.ShippingAddress.PostalCode,
                    Name = orderDto.ShippingAddress.Name,
                    Phonenumber = orderDto.ShippingAddress.Phonenumber
                }),

                // Map PickupLocation JSON
                PickupLocation = JsonConvert.SerializeObject(new PickUpLocation
                {
                    countyId = orderDto.PickUpLocation.countyId,
                    townId = orderDto.PickUpLocation.townId,
                    deliveryStationId = orderDto.PickUpLocation.deliveryStationId
                })
            };
        }
    }
}
