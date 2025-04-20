using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimart_Api.Data;
using Minimart_Api.DTOS;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Services.RabbitMQ;
using Minimart_Api.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Minimart_Api.Repositories
{
    public class MyRepository : IRepository
    {
        private readonly MinimartDBContext _dbContext;

        private readonly IConfiguration _configuration;

        private readonly MpesaSandBox _mpesaSandBox;

        private readonly IOrderEventPublisher _orderEventPublisher;

        private readonly ILogger<MyRepository> _logger;

        public MyRepository(MinimartDBContext myDBContext,
            IConfiguration configuration,
            IOptions<MpesaSandBox> mpesaSandBox, 
            IOrderEventPublisher orderEventPublisher,
            ILogger<MyRepository> logger)
        {
            _dbContext = myDBContext;
            _configuration = configuration;
            _mpesaSandBox = mpesaSandBox.Value;
            _orderEventPublisher = orderEventPublisher;
            _logger = logger;
        }

        public async Task<IEnumerable<Users>> GetAllAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }

        public async Task<IEnumerable<Users>> GetAsyncUserName(string userName)
        {
            return await _dbContext.Users.Where(u => u.UserName == userName).ToListAsync();
        }

        



        //public async Task<UserRegStatus> UserRegistration(string jsonData)
        //{
        //    var parameters = new[]
        //    {
        //        new SqlParameter("@JsonData", jsonData)
        //    };

        //    var response = await _dbContext.UsrRegStatuses.FromSqlRaw("EXEC p_AddUser @JsonData", parameters).ToListAsync();
        //    return response.FirstOrDefault();
        //}

        //public async Task<LoginResponse> Login(string jsonData)
        //{
        //    var parameters = new[]
        //    {
        //        new SqlParameter("@JsonData", jsonData)
        //    };

        //    var response = await _dbContext.Users.FromSqlRaw("EXEC p_AuthenticateUser @JsonData", parameters).ToListAsync();
        //    return response.FirstOrDefault();
        //}

        public async Task SaveRefreshToken(string jsonData)
        {
            var parameters = new[]
            {
                new SqlParameter("@jsonData", jsonData)
            };

            await _dbContext.Database.ExecuteSqlRawAsync("EXEC p_SaveRefreshToken @jsonData", parameters);
        }

        //public async Task<IEnumerable<CategoryDTO>> GetDashBoardCategories()
        //{
        //    return await _dbContext.TCategories
        //        .Include(tc => tc.TSubcategoryids)
        //        .Select(tc => new CategoryDTO
        //        {
        //            Id = tc.CategoryId,
        //            Name = tc.CategoryName,
        //            Description = tc.Description,
        //            Subcategoryids = tc.TSubcategoryids
        //                .Select(sc => new SubCategoryDTO
        //                {
        //                    Id = sc.SubCategoryId,
        //                    Name = sc.ProductName,
        //                })
        //                .ToList()
        //        })
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName dashboardName)
        //{
        //    return await _dbContext.TSubcategoryids.Where(p => p.ProductName == dashboardName.Name).ToListAsync();
        //}




        public async Task<UserInfo> GetRefreshToken(string userID)
        {

            try
            {
                var response = await  _dbContext.Users
                    .Where(u => u.UserId == Convert.ToInt32(userID))
                    .Select(u => new UserInfo
                    {
                        UserInfoId = u.UserId,
                        Name = u.UserName,
                        Email = u.Email,
                        RefreshToken = u.RefreshToken,
                        phonenumber = u.PhoneNumber,
                        Password = u.Password,
                        RoleID = u.RoleId,
                        StatusId = 1,
                        Status = new LoginResponseStatus { 
                            ResponseCode = true,
                            ResponseStatusId = 200,
                            ResponseMessage = "Refresh Token Retrieved Succesfully"
                        }
                    }).FirstOrDefaultAsync();

                return response;

            }
            catch (Exception ex) {
                _logger.LogInformation($"Error in retrieving Refresh Token, {ex.Message}");
                return new UserInfo();
            } 
    
        }








        //public async Task<ResponseStatus> AddOrder(OrderListDto transaction)
        //{
        //    using var transactionScope = await _dbContext.Database.BeginTransactionAsync();

        //    int paymentMethodID = 0;
        //    var newOrder = new Order();

        //    try
        //    {
        //        // Iterate through each order in the transaction
        //        foreach (var orderDto in transaction.Orders)
        //        {
        //            // Check if PaymentID exists in the Payments table
        //            foreach (var paymentDetailDto in orderDto.PaymentDetails)
        //            {
        //                var paymentExists = _dbContext.payments
        //                    .Any(p => p.PaymentID == paymentDetailDto.PaymentID);

        //                if (!paymentExists)
        //                {
        //                    // Handle the case where PaymentID does not exist
        //                    throw new Exception($"PaymentID {paymentDetailDto.PaymentID} does not exist in the Payments table.");
        //                }

        //                if (paymentDetailDto.PaymentMethod == "Mpesa")
        //                {
        //                    var stkPushResponse = await InitiateMpesaSTKPush(paymentDetailDto);

        //                    if (stkPushResponse.ResponseCode == "1" )
        //                    {
        //                        return new ResponseStatus
        //                        {
        //                            ResponseStatusId = 400,
        //                            ResponseMessage = "M-Pesa STK Push failed: " + stkPushResponse.CustomerMessage
        //                        };
        //                    }

        //                    // Save STK Push details for auditing
        //                    var newPayment = new PaymentDetails
        //                    {
        //                        PaymentID = paymentDetailDto.PaymentID,
        //                        TrxReference = stkPushResponse.CheckoutRequestID, // Use CheckoutRequestID from the response
        //                        Amount = paymentDetailDto.Amount,
        //                        PaymentDate = DateTime.UtcNow,
        //                        Phonenumber = paymentDetailDto.Phonenumber,

        //                    };

        //                    _dbContext.paymentDetails.Add(newPayment);
        //                    await _dbContext.SaveChangesAsync();

        //                    // Fetch the PaymentMethodID for use in the order
        //                    paymentMethodID = newPayment.PaymentMethodID;
        //                }

        //                // Proceed with the order creation
        //                newOrder = new Order
        //                {
        //                    OrderID = orderDto.OrderID,
        //                    UserID = orderDto.UserID,
        //                    OrderDate = orderDto.OrderDate,
        //                    DeliveryScheduleDate = orderDto.DeliveryScheduleDate,
        //                    OrderedBy = orderDto.OrderedBy,
        //                    Status = orderDto.Status,
        //                    PaymentMethodID = paymentMethodID, // Assign the correct PaymentMethodID here
        //                    PaymentConfirmation = orderDto.PaymentConfirmation,
        //                    TotalOrderAmount = orderDto.TotalOrderAmount,
        //                    TotalPaymentAmount = orderDto.TotalPaymentAmount,
        //                    TotalDeliveryFees = orderDto.TotalDeliveryFees,
        //                    TotalTax = 0,

        //                    // Map PaymentDetails JSON
        //                    PaymentDetailsJson = JsonConvert.SerializeObject(new
        //                    {
        //                        PaymentID = paymentDetailDto.PaymentID,
        //                        PaymentReference = paymentDetailDto.PaymentReference,
        //                        Amount = paymentDetailDto.Amount,
        //                        PaymentDate = DateTime.UtcNow // Use the current date
        //                    }),

        //                    // Map Products JSON
        //                    ProductsJson = JsonConvert.SerializeObject(orderDto.Products.Select(p => new
        //                    {
        //                        ProductName = p.ProductName,
        //                        ProductID = p.ProductID,
        //                        Quantity = p.Quantity,
        //                        Price = p.Price,
        //                        Discount = p.Discount
        //                    }).ToList()),

        //                    // Map the collection of OrderProducts
        //                    OrderProducts = orderDto.Products.Select(productDto => new OrderProducts
        //                    {
        //                        ProductID = productDto.ProductID,
        //                        Quantity = productDto.Quantity,
        //                    }).ToList(),

        //                    // Map ShippingAddress JSON
        //                    ShippingAddress = JsonConvert.SerializeObject(new ShippingAddress
        //                    {
        //                        Address = orderDto.ShippingAddress.Address,
        //                        County = orderDto.ShippingAddress.County,
        //                        Town = orderDto.ShippingAddress.Town,
        //                        PostalCode = orderDto.ShippingAddress.PostalCode,
        //                        Name = orderDto.ShippingAddress.Name,
        //                        Phonenumber = orderDto.ShippingAddress.Phonenumber
        //                    }),

        //                    // Map PickupLocation JSON
        //                    PickupLocation = JsonConvert.SerializeObject(new PickUpLocation
        //                    {
        //                        countyId = orderDto.PickUpLocation.countyId,
        //                        townId = orderDto.PickUpLocation.townId,
        //                        deliveryStationId = orderDto.PickUpLocation.deliveryStationId
        //                    })
        //                };

        //                // Update product quantities in stock
        //                foreach (var product in orderDto.Products)
        //                {
        //                    var existingProduct = await _dbContext.TProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
        //                    if (existingProduct != null)
        //                    {
        //                        // Reduce product quantity
        //                        existingProduct.InStock -= product.Quantity;

        //                        // Prevent negative quantity
        //                        if (existingProduct.InStock < 0)
        //                        {
        //                            return new ResponseStatus
        //                            {
        //                                ResponseStatusId = 400,
        //                                ResponseMessage = $"Insufficient stock for product: {existingProduct.ProductName}"
        //                            };
        //                        }

        //                        // Detach the entity from EF tracking to prevent EF from trying to update the RowID
        //                        _dbContext.Entry(existingProduct).State = EntityState.Detached;

        //                        // Attach entity and explicitly set the properties to update
        //                        _dbContext.TProducts.Attach(existingProduct);
        //                        _dbContext.Entry(existingProduct).Property(x => x.InStock).IsModified = true;
        //                    }
        //                }



        //                // Add the new order to the context
        //                _dbContext.orders.Add(newOrder);
        //            }
        //        }

        //        // Save all changes to the database
        //        await _dbContext.SaveChangesAsync();
        //        await transactionScope.CommitAsync();

        //       //await  PublishOrderEvent(newOrder);

        //        return new ResponseStatus
        //        {
        //            ResponseStatusId = 200,
        //            ResponseMessage = "Transaction completed successfully"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await transactionScope.RollbackAsync();

        //        return new ResponseStatus
        //        {
        //            ResponseStatusId = 500,
        //            ResponseMessage = $"Internal Server Error: {ex.Message}"
        //        };
        //    }
        //}

        private async Task<STKPushResponse> InitiateMpesaSTKPush(PaymentDetailsDto paymentDetails)
        {
            string token = string.Empty;

            try
            {
                var newSTKPushRequest = new STKPushRequest
                {
                    BusinessShortCode = "174379",
                    Password = GeneratePassword(),
                    Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                    TransactionType = "CustomerPayBillOnline",
                    Amount = paymentDetails.Amount,
                    PartyA = Convert.ToString(paymentDetails.PaymentReference),
                    PartyB = "174379",
                    PhoneNumber = Convert.ToString(paymentDetails.PaymentReference),
                    CallBackURL = "https://ce19-102-213-49-29.ngrok-free.app/mpesa/callback",
                    AccountReference = $"Order{paymentDetails.PaymentID}",
                    TransactionDesc = $"Payment For {paymentDetails.PaymentID}",
                    //PassKey = "bfb279f9aa9bdbcf158e97dd71a467cd2f54f2a74b1cfcfc9e68d8f7cbe72956

                };

                var json = JsonConvert.SerializeObject(newSTKPushRequest);

                var ConsumerKey = _mpesaSandBox.ConsumerKey;
                var ConsumerSecret = _mpesaSandBox.ConsumerSecret;
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConsumerKey}:{ConsumerSecret}"))
                );

                var Authresponse = await client.GetAsync(_mpesaSandBox.MpesaSandboxUrl);

                if (Authresponse.IsSuccessStatusCode)
                {
                    var content = await Authresponse.Content.ReadAsStringAsync();

                    var data = JsonConvert.DeserializeObject<dynamic>(content);

                    token = data?["access_token"]?.ToString() ?? throw new InvalidOperationException();
                }
                else
                {
                    throw new HttpRequestException($"Failed to get access token. Status Code: {Authresponse.StatusCode}");
                }

                // Send STK PUSH REQUEST
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.PostAsJsonAsync(_mpesaSandBox.STKPushUrl, newSTKPushRequest);
                var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    return new STKPushResponse
                    {
                        MerchantRequestID = responseData.MerchantRequestID,
                        CheckoutRequestID = responseData.CheckoutRequestID,
                        ResponseCode = responseData.ResponseCode,
                        ResponseDescription = responseData.ResponseDescription,
                        CustomerMessage = responseData.CustomerMessage,
                    };
                }
                else
                {
                    // Return a failure response if the API call fails
                    return new STKPushResponse
                    {
                        MerchantRequestID = "",
                        CheckoutRequestID = "",
                        ResponseCode = responseData?.ResponseCode ?? "1",
                        ResponseDescription = responseData?.ResponseDescription ?? "Failed to initiate STK push.",
                        CustomerMessage = responseData?.CustomerMessage ?? "Failed to initiate STK push.",
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle the exception by returning a generic error response
                return new STKPushResponse
                {
                    MerchantRequestID = "",
                    CheckoutRequestID = "",
                    ResponseCode = "1",
                    ResponseDescription = "An error occurred while initiating the STK push.",
                    CustomerMessage = "An error occurred while initiating the STK push.",
                };
            }
        }


        private string GeneratePassword()
        {
            var shortcode = "174379"; // Replace with your shortcode
            var passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2f54f2a74b1cfcfc9e68d8f7cbe72956"; // Replace with your passkey
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var dataToEncode = shortcode + passkey + timestamp;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(dataToEncode));
        }

        //PUBLISH TO ORDEREVENT
        //public async Task PublishOrderEvent(Order order) {

        //    //create an order Event Object
        //    var orderEvent = new OrderEvent
        //    {
        //        OrderID = order.OrderID,
        //        OrderDate = order.OrderDate,
        //        UserID = order.UserID,
        //        products = order.ProductsJson,
        //        UserEmail = "user@gmail.com",
        //        MerchantEmail = "merchant@gmail.com",
        //        UserPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
        //        MerchantPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
        //        addresses = order.ShippingAddress,
        //        Amount = order.TotalPaymentAmount
        //    };

        //    await _orderEventPublisher.PublishOrderEvent(orderEvent);
        //}


       


      


        //public async Task<ResponseStatus> CreateOrder(Order order)
        //{
        //    using (var transaction = await _dbContext.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            // Deserialize PaymentDetails from JSON
        //            var newPayment = JsonConvert.DeserializeObject<PaymentDetails>(order.PaymentDetailsJson);

        //            if (newPayment != null)
        //            {
        //                // Ensure the payment date is valid
        //                if (newPayment.PaymentDate == DateTime.MinValue)
        //                {
        //                    newPayment.PaymentDate = DateTime.Now;
        //                }

        //                // Add payment details to the context
        //                _dbContext.paymentDetails.Add(newPayment);
        //                await _dbContext.SaveChangesAsync();
        //            }

        //            // Update product stock quantities based on the order
        //            foreach (var product in order.OrderProducts)
        //            {
        //                var existingProduct = await _dbContext.TProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
        //                if (existingProduct != null)
        //                {
        //                    // Check if the stock is sufficient
        //                    if (existingProduct.InStock < product.Quantity)
        //                    {
        //                        return new ResponseStatus
        //                        {
        //                            ResponseStatusId = 400,
        //                            ResponseMessage = $"Insufficient stock for product ID: {product.ProductID}"
        //                        };
        //                    }

        //                    existingProduct.InStock -= product.Quantity;
        //                    _dbContext.TProducts.Update(existingProduct);
        //                }
        //                else
        //                {
        //                    return new ResponseStatus
        //                    {
        //                        ResponseStatusId = 404,
        //                        ResponseMessage = $"Product ID: {product.ProductID} not found."
        //                    };
        //                }
        //            }

        //            // Add the order to the context
        //            _dbContext.orders.Add(order);
        //            await _dbContext.SaveChangesAsync();

        //            // Commit the transaction
        //            await transaction.CommitAsync();

        //            return new ResponseStatus
        //            {
        //                ResponseStatusId = 200,
        //                ResponseMessage = "Transaction completed successfully"
        //            };
        //        }
        //        catch (DbUpdateConcurrencyException ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return new ResponseStatus
        //            {
        //                ResponseStatusId = 409,
        //                ResponseMessage = $"Concurrency error: {ex.Message}"
        //            };
        //        }
        //        catch (DbUpdateException ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return new ResponseStatus
        //            {
        //                ResponseStatusId = 500,
        //                ResponseMessage = $"Database update error: {ex.Message} - Inner: {ex.InnerException?.Message}"
        //            };
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return new ResponseStatus
        //            {
        //                ResponseStatusId = 500,
        //                ResponseMessage = $"Internal Server Error: {ex.Message} - Inner: {ex.InnerException?.Message}"
        //            };
        //        }
        //    }
        //}




     
    }
}
