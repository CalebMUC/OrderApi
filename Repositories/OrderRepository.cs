using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.Repositories;
using Minimart_Api.TempModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OrderRepository : IorderRepository
{
    private readonly MinimartDBContext _dbContext;

    public OrderRepository(MinimartDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status,int userID)
    {
        try
        {
            // Use a join query to get the orders and their status messages
            var orders = await _dbContext.orders
                .Where(o => o.Status == status
                    && o.UserID == userID)
                .Join(_dbContext.orderStatus,
                      o => o.Status,
                      os => os.Status,
                      (o, os) => new GetOrdersDTO
                      {
                          OrderID = o.OrderID,
                          OrderDate = o.OrderDate,
                          TotalOrderAmount = o.TotalOrderAmount,
                          Status = os.StatusMessage,

                          PaymentConfirmation = o.PaymentConfirmation,
                          TotalPaymentAmount = o.TotalPaymentAmount,
                          TotalDeliveryFees = o.TotalDeliveryFees,
                          TotalTax = o.TotalTax,
                          ShippingAddress = JsonConvert.DeserializeObject<ShippingAddress>(o.ShippingAddress),
                          Products = JsonConvert.DeserializeObject<List<OrderProductsDTO>>(o.ProductsJson),

                          PickUpLocation = JsonConvert.DeserializeObject<PickUpLocation>(o.PickupLocation),
                          PaymentDetails = JsonConvert.DeserializeObject<PaymentDetailsDto>(o.PaymentDetailsJson)


        })
                .ToListAsync();

            return orders;
        }
        catch (Exception ex) {
            return [];
        }
    }
}
