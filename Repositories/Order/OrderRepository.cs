using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Minimart_Api.Data;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.General;
using Minimart_Api.DTOS.Merchants;
using Minimart_Api.DTOS.Orders;
using Minimart_Api.DTOS.Payments;
using Minimart_Api.DTOS.Products;
using Minimart_Api.Models;
using Minimart_Api.Repositories.Order;
using Minimart_Api.Services.RabbitMQ;
using Minimart_Api.Services.SignalR;
using Minimart_Api.Services.SystemMerchantService;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class OrderRepository : IorderRepository
{
    private readonly MinimartDBContext _dbContext;

    private readonly IOrderEventPublisher _orderEventPublisher;

    private readonly IConfiguration _configuration;

    private readonly MpesaSandBox _mpesaSandBox;

    private readonly IHubContext<ActivityHub> _hubContext;

    private readonly ISystemMerchants _systemMerchants;

    private readonly IHttpClientFactory _clientFactory;
    private const string ConsumerKey = "vM5KjasAGTVzdddzpP8tENa1Z9us6G6CDjeZzEAHQKzVbQu4";
    private const string ConsumerSecret = "BZQ2uAq84LIzonV6uaXBo7ofYGTHvhhvFD5vVd8EuTwnsd0n0b9ewQ8ExNMKuOnn";
    private const string BusinessShortCode = "174379";
    private const string PassKey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
    public OrderRepository(MinimartDBContext dbContext,
        IOrderEventPublisher orderEventPublisher,
        IOptions<MpesaSandBox> mpesaSandBox,
        IHttpClientFactory clientFactory,
        IHubContext<ActivityHub> hubContext,
        ISystemMerchants systemMerchants
        /*IConfiguration configuration*/)
    {
        _dbContext = dbContext;
        _orderEventPublisher = orderEventPublisher;
        //_configuration = configuration;
        _mpesaSandBox = mpesaSandBox.Value;
        _clientFactory = clientFactory;
        _systemMerchants = systemMerchants;

    }

    public async Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID)
    {
        try
        {
            // Step 1: Fetch orders with status messages
            var ordersWithStatus = await _dbContext.Orders
                .Where(o => o.Status == status && o.UserID == userID)
                .Join(_dbContext.OrderStatuses,
                    o => o.Status, // Join condition for order status
                    os => os.StatusId,
                    (o, os) => new { Order = o, StatusMessage = os.Status })
                .ToListAsync();

            // Step 2: Map the result to GetOrdersDTO and fetch product images
            var orders = new List<GetOrdersDTO>();

            foreach (var orderWithStatus in ordersWithStatus)
            {
                var order = orderWithStatus.Order;
                var products = JsonConvert.DeserializeObject<List<OrderProductsDTO>>(order.ProductsJson);

                // Fetch ImageUrl for each product
                var productsWithImages = products.Select(p => new OrderProductsDTO
                {
                    ProductID = p.ProductID,
                    ProductName = p.ProductName,
                    Quantity = p.Quantity,
                    Price = p.Price,
                    ImageUrl = _dbContext.Products
                        .FirstOrDefault(tp => tp.ProductId == p.ProductID)?.ImageUrl // Fetch ImageUrl for each product
                }).ToList();

                // Map to GetOrdersDTO
                var getOrderDTO = new GetOrdersDTO
                {
                    OrderID = order.OrderID,
                    OrderDate = order.OrderDate,
                    TotalOrderAmount = order.TotalOrderAmount,
                    Status = orderWithStatus.StatusMessage,

                    PaymentConfirmation = order.PaymentConfirmation,
                    TotalPaymentAmount = order.TotalPaymentAmount,
                    TotalDeliveryFees = order.TotalDeliveryFees,
                    TotalTax = order.TotalTax,
                    ShippingAddress = JsonConvert.DeserializeObject<ShippingAddress>(order.ShippingAddress),
                    Products = productsWithImages, // Include products with ImageUrl

                    PickUpLocation = JsonConvert.DeserializeObject<PickUpLocation>(order.PickupLocation),
                    PaymentDetails = JsonConvert.DeserializeObject<List<PaymentDetailsDto>>(order.PaymentDetailsJson)
                };

                orders.Add(getOrderDTO);
            }

            return orders;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            return [];
        }
    }

    public async Task<List<OrderStatus>> GetOrderStatusAsync()
    {
        try
        {
            // Execute the query asynchronously and materialize the results into a list
            var orderStatusList = await _dbContext.OrderStatuses
                .Select(o => new OrderStatus
                {
                    StatusId = o.StatusId,
                    Status = o.Status,
                    Order = o.Order,
                    Description = o.Description,
                    CreatedBy = o.CreatedBy,
                    CreatedOn = o.CreatedOn,
                    UpdatedBy = o.UpdatedBy,
                    UpdatedOn = o.UpdatedOn,
                })
                .ToListAsync(); // Use ToListAsync to execute the query asynchronously

            return orderStatusList;
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            // _logger.LogError(ex, "An error occurred while fetching order statuses.");

            // Return an empty list in case of an error
            return new List<OrderStatus>();
        }
    }


    public async Task<List<OrderTracking>> GetOrderTrackingAsync(GetOrderTrackingStatus trackingStatus)
    {
        try
        {
            var tracking = await _dbContext.OrderTrackings
                .Where(ot => ot.ProductID == trackingStatus.ProductID)
                .Select(ot => new OrderTracking
                {
                    TrackingID = ot.TrackingID,
                    OrderID = ot.OrderID,
                    ProductID = ot.ProductID,
                    CurrentStatus = ot.CurrentStatus,
                    PreviousStatus = ot.PreviousStatus,
                    TrackingDate = ot.TrackingDate,
                    ExpectedDeliveryDate = ot.ExpectedDeliveryDate, // Update this if needed
                    Carrier = ot.Carrier,
                    CreatedOn = ot.CreatedOn,
                    CreatedBy = ot.CreatedBy,
                    UpdatedBy = ot.UpdatedBy,
                    UpdatedOn = ot.UpdatedOn,
                }).ToListAsync();

            return tracking;
        }
        catch (Exception ex) { 
            return new List<OrderTracking>();
        }
    }



    public async Task<List<GetOrdersDTO>> GetOrdersByIdAsync(string OrderId)
    {
        try
        {
            // Use a join query to get the orders and their status messages
            var orders = await _dbContext.Orders
                .Where(o => o.OrderID == OrderId
)
                .Join(_dbContext.OrderStatuses,
                      o => o.Status,
                      os => os.StatusId,
                      (o, os) => new GetOrdersDTO
                      {
                          OrderID = o.OrderID,
                          OrderDate = o.OrderDate,
                          TotalOrderAmount = o.TotalOrderAmount,
                          Status = os.Status,

                          PaymentConfirmation = o.PaymentConfirmation,
                          TotalPaymentAmount = o.TotalPaymentAmount,
                          TotalDeliveryFees = o.TotalDeliveryFees,
                          TotalTax = o.TotalTax,
                          ShippingAddress = JsonConvert.DeserializeObject<ShippingAddress>(o.ShippingAddress),
                          Products = JsonConvert.DeserializeObject<List<OrderProductsDTO>>(o.ProductsJson),

                          PickUpLocation = JsonConvert.DeserializeObject<PickUpLocation>(o.PickupLocation),
                          PaymentDetails = JsonConvert.DeserializeObject<List<PaymentDetailsDto>>(o.PaymentDetailsJson) // Deserialize as List
                      })
                .ToListAsync();

            return orders;
        }
        catch (Exception ex)
        {
            // Log the exception (ex) here if needed
            return [];
        }
    }

    public async Task<List<MerchantOrderDto>> GetAdminOrdersAsync()
    {
        try
        {
            var merchantOrders = _dbContext.Orders
                .Join(
                    _dbContext.OrderStatuses,
                    o => o.Status,
                    os => os.StatusId,
                    (o, os) => new { Order = o, StatusName = os.Status }
                )
                .AsEnumerable() // Forces execution of the query in memory
                .SelectMany(joined =>
                    JsonConvert.DeserializeObject<List<OrderProductsDTO>>(joined.Order.ProductsJson)
                    .Select(p => new MerchantOrderDto
                    {
                        OrderId = joined.Order.OrderID,
                        ProductName = p.ProductName,
                        Quantity = p.Quantity,
                        Price = p.Price,
                        Status = joined.StatusName
                    })
                )
                .ToList(); // Use synchronous ToList()

            return merchantOrders;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<MerchantOrderDto>(); // Return an empty list instead of throwing
        }
    }


    public async Task<List<MerchantOrderDto>> GetMerchantOrdersAsync(MerchantRequestDto requestDto)
    {

        try
        {
            // Apply filtering at the database level
            var query = _dbContext.Orders
                .Join(
                    _dbContext.OrderStatuses,
                    o => o.Status,
                    os => os.StatusId,
                    (o, os) => new { Order = o, StatusName = os.Status }
                )
                .Where(joined => joined.Order.ProductsJson != null); // Ensure ProductsJson is not null before processing

            if (!string.IsNullOrEmpty(requestDto.OrderId))
            {
                query = query.Where(joined => joined.Order.OrderID == requestDto.OrderId);
            }

            var rawOrders = await query.ToListAsync(); // Fetch filtered orders from DB

            var merchantOrders = rawOrders
                .SelectMany(joined =>
                    JsonConvert.DeserializeObject<List<OrderProductsDTO>>(joined.Order.ProductsJson ?? "[]") // Handle null JSON
                    .Where(p => p.merchantId == requestDto.MerchantId) // Filter by merchantId
                    .Select(p => new MerchantOrderDto
                    {
                        OrderId = joined.Order.OrderID,
                        Quantity = p.Quantity,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        Status = joined.StatusName,
                        ProductID = p.ProductID
                    })
                )
                .ToList(); // Convert to list in memory

            return merchantOrders;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            return new List<MerchantOrderDto>(); // Return an empty list instead of throwing
        }
    }

    public async Task<Status> UpdateOrderStatusAsync(OrderTrackingDTO orderTracking)
    {
        try
        {
            // Get Existing Tracking Record
            var existingProductTracker = await _dbContext.OrderTrackings
                .Where(ot => ot.OrderID == orderTracking.OrderId && ot.ProductID == orderTracking.ProductId)
                .FirstOrDefaultAsync();

            // Check if the record exists
            if (existingProductTracker == null)
            {
                return new Status
                {
                    ResponseCode = 404,
                    ResponseMessage = "Order tracking record not found."
                };
            }

            // Update Existing Tracking Record
            existingProductTracker.PreviousStatus = existingProductTracker.CurrentStatus;
            existingProductTracker.CurrentStatus = orderTracking.StatusId; // Assuming DTO has NewStatusId
            existingProductTracker.UpdatedBy = orderTracking.UpdatedBy; // Assuming DTO has UpdatedBy
            existingProductTracker.UpdatedOn = DateTime.Now;

            // Save Changes
            _dbContext.OrderTrackings.Update(existingProductTracker);
            await _dbContext.SaveChangesAsync();

            return new Status
            {
                ResponseCode = 200,
                ResponseMessage = "Order Tracking Updated Successfully"
            };
        }
        catch (Exception ex)
        {
            return new Status
            {
                ResponseCode = 500,
                ResponseMessage = ex.Message
            };
        }
    }


    public async Task<Status> TrackOrderAsync(Orders order)
    {
        try
        {
            var statusId = await _dbContext.OrderStatuses
                .Where(os => os.Status == "Processing")
                .Select(os => os.StatusId)
                .FirstOrDefaultAsync();

            var createdBy = await _dbContext.Users
                .Where(u => u.UserId == order.UserID)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            // Loop through each product in the order
            foreach (var product in order.OrderProducts)
            {
                var trackingId = $"TRK-{Guid.NewGuid().ToString().Substring(0, 4)}";

                var newOrderTrack = new OrderTracking
                {
                    TrackingID = trackingId,
                    OrderID = order.OrderID,
                    ProductID = product.ProductID,
                    CurrentStatus = statusId,
                    PreviousStatus = statusId,
                    TrackingDate = DateTime.Now,
                    ExpectedDeliveryDate = DateTime.Now, // Update this if needed
                    Carrier = "ABC Delivery Company",
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    UpdatedBy = "",
                    UpdatedOn = DateTime.Now
                };

                // Add each tracking record to the DbContext
                _dbContext.OrderTrackings.Add(newOrderTrack);
            }

            // Save all tracking records after the loop
            await _dbContext.SaveChangesAsync();

            return new Status
            {
                ResponseCode = 200,
                ResponseMessage = "Order Tracking Created Successfully for All Products"
            };
        }
        catch (Exception ex)
        {
            return new Status
            {
                ResponseCode = 500,
                ResponseMessage = ex.Message
            };
        }
    }





    //Create Order
    public async Task<Status> AddOrder(OrderListDto transaction)
    {
        using var transactionScope = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            foreach (var orderDto in transaction.Orders)
            {
                int paymentMethodID = await HandlePaymentDetails(orderDto.PaymentDetails);

                var newOrder = await CreateOrder(orderDto, paymentMethodID);

                await UpdateProductStock(orderDto.Products);

                // Save the order and commit the transaction
                _dbContext.Orders.Add(newOrder);
                await _dbContext.SaveChangesAsync();

                //update CartItems To BoughtItems
                await UpdateCartItems(orderDto.Products,newOrder.UserID);

                await TrackOrderAsync(newOrder);

                await PublishOrderEvent(newOrder);

                //_hubContext.Clients.All.SendAsync("ReceiveNewOrder", $"New OrderId {newOrder.OrderID} has been created");
            }

            await transactionScope.CommitAsync();

            

            return new Status
            {
                ResponseCode = 200,
                ResponseMessage = "Transaction completed successfully"
            };
        }
        catch (Exception ex)
        {
            await transactionScope.RollbackAsync();

            return new Status
            {
                ResponseCode = 500,
                ResponseMessage = $"Internal Server Error: {ex.Message}"
            };
        }
    }

    private async Task<int> HandlePaymentDetails(List<PaymentDetailsDto> paymentDetails)
    {
        int paymentMethodID = 0;

        foreach (var paymentDetailDto in paymentDetails)
        {
            var paymentExists = _dbContext.PaymentMethods
                .Any(p => p.PaymentMethodID == paymentDetailDto.PaymentID);

            if (!paymentExists)
            {
                throw new Exception($"PaymentID {paymentDetailDto.PaymentID} does not exist in the Payments table.");
            }

            if (paymentDetailDto.PaymentMethod == "Mpesa")
            {
                var stkPushResponse = await InitiateMpesaSTKPush(paymentDetailDto);

                if (stkPushResponse.ResponseCode == "1")
                {
                    throw new Exception("M-Pesa STK Push failed: " + stkPushResponse.CustomerMessage);
                }

                var newPayment = new PaymentDetails
                {
                    PaymentMethodID = paymentDetailDto.PaymentID,
                    TrxReference = stkPushResponse.CheckoutRequestID,
                    Amount = paymentDetailDto.Amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentReference = Convert.ToString(paymentDetailDto.Phonenumber)
                };

                _dbContext.PaymentDetails.Add(newPayment);
                await _dbContext.SaveChangesAsync();

                paymentMethodID = newPayment.PaymentMethodID;
            }
        }

        return paymentMethodID;
    }

    private async Task<Orders> CreateOrder(OrderDTO orderDto, int paymentMethodID)
    {
        return new Orders
        {
            OrderID = orderDto.OrderID,
            //MerchantId = orderDto.MerchantId,
            UserID = orderDto.UserID,
            OrderDate = DateTime.Now,
            DeliveryScheduleDate = orderDto.DeliveryScheduleDate,
            OrderedBy = orderDto.OrderedBy,
            Status = orderDto.Status,
            PaymentID = paymentMethodID,
            PaymentConfirmation = orderDto.PaymentConfirmation,
            TotalOrderAmount = orderDto.TotalOrderAmount,
            TotalPaymentAmount = orderDto.TotalPaymentAmount,
            TotalDeliveryFees = orderDto.TotalDeliveryFees,
            TotalTax = 0,
            PaymentDetailsJson = JsonConvert.SerializeObject(orderDto.PaymentDetails),
            ProductsJson = JsonConvert.SerializeObject(orderDto.Products),
            OrderProducts = orderDto.Products.Select(p => new OrderProducts
            {
                ProductID = p.ProductID,
                Quantity = p.Quantity
            }).ToList(),
            ShippingAddress = JsonConvert.SerializeObject(orderDto.ShippingAddress),
            PickupLocation = JsonConvert.SerializeObject(orderDto.PickUpLocation),
            
        };
    }
    private async Task UpdateProductStock(List<OrderProductsDTO> products)
    {
        foreach (var product in products)
        {
            var existingProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
            if (existingProduct != null)
            {
                existingProduct.StockQuantity -= product.Quantity;

                if (existingProduct.StockQuantity < 0)
                {
                    throw new Exception($"Insufficient stock for product: {existingProduct.ProductName}");
                }

                //detach entity for EF tracking to prevent updating RowID
                _dbContext.Entry(existingProduct).State = EntityState.Detached;

                //attach and explicitly set properties to update

                _dbContext.Products.Attach(existingProduct);

                _dbContext.Entry(existingProduct).Property(x => x.InStock).IsModified = true;
            }
        }
    }


    private async Task UpdateCartItems(List<OrderProductsDTO> products,int UserId)
    {
        // Get all product IDs from the order
        var productIds = products.Select(p => p.ProductID).ToList();

        // Fetch all relevant cart items in a single query
        var cartItemsToUpdate = await _dbContext.CartItems
                            .Where(c => productIds.Contains(c.ProductId) &&
                                        c.Cart.UserId == UserId) // Assuming you have this relationship
                            .Select( C => new CartItem { 
                                CartItemId = C.CartItemId,
                                CartId = C.CartId,
                                ProductId =C.ProductId,
                                Quantity = C.Quantity,
                                CreatedOn = C.CreatedOn,
                                UpdatedOn = C.UpdatedOn,
                                IsBought = C.IsBought,
                                IsActive = C.IsActive
                            })
                            .ToListAsync();

        foreach (var cartItem in cartItemsToUpdate)
        {
            // Use a more efficient way to update without detaching/attaching
            cartItem.IsActive = false;
            cartItem.IsBought = true;
            cartItem.UpdatedOn = DateTime.UtcNow;

            // Mark as modified (only needed if you're not tracking these entities)
            _dbContext.Entry(cartItem).State = EntityState.Modified;
        }

        // Single SaveChanges call for all updates
        await _dbContext.SaveChangesAsync();
    }

    //private async Task<STKPushResponse> InitiateMpesaSTKPush(PaymentDetailsDto paymentDetails)
    //{
    //    string token = string.Empty;

    //    try
    //    {
    //        var generatedPassword = GeneratePassword();
    //        var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
    //        var payLoad = new 
    //        {
    //            BusinessShortCode = "174379",
    //            Password = "MTc0Mzc5YmZiMjc5ZjlhYTliZGJjZjE1OGU5N2RkNzFhNDY3Y2QyZTBjODkzMDU5YjEwZjc4ZTZiNzJhZGExZWQyYzkxOTIwMjUwMjA1MDU1ODMx",
    //            Timestamp = timeStamp,
    //            TransactionType = "CustomerPayBillOnline",
    //            Amount = 1,
    //            PartyA = "254794129559",
    //            PartyB = "174379",
    //            PhoneNumber = "254794129559",
    //            CallBackURL = "https://mydomain.com/path",
    //            AccountReference = "Test123",
    //            TransactionDesc = "Test Payment"

    //        };



    //        var json = JsonConvert.SerializeObject(payLoad);

    //        var ConsumerKey = _mpesaSandBox.ConsumerKey;
    //        var ConsumerSecret = _mpesaSandBox.ConsumerSecret;
    //        var client = new HttpClient();
    //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
    //            "Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConsumerKey}:{ConsumerSecret}"))
    //        );

    //        var Authresponse = await client.GetAsync(_mpesaSandBox.MpesaSandboxUrl);

    //        if (Authresponse.IsSuccessStatusCode)
    //        {
    //            var content = await Authresponse.Content.ReadAsStringAsync();

    //            var data = JsonConvert.DeserializeObject<dynamic>(content);

    //            token = data?["access_token"]?.ToString() ?? throw new InvalidOperationException();
    //        }
    //        else
    //        {
    //            throw new HttpRequestException($"Failed to get access token. Status Code: {Authresponse.StatusCode}");
    //        }

    //        // Send STK PUSH REQUEST
    //        client.DefaultRequestHeaders.Accept.Clear();
    //        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    //        var payloadContent = new StringContent(json, Encoding.UTF8, "application/json");

    //        var response = await client.PostAsJsonAsync(_mpesaSandBox.STKPushUrl, payloadContent);
    //        var responseData = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

    //        if (response.IsSuccessStatusCode)
    //        {
    //            return new STKPushResponse
    //            {
    //                MerchantRequestID = responseData.MerchantRequestID,
    //                CheckoutRequestID = responseData.CheckoutRequestID,
    //                ResponseCode = responseData.ResponseCode,
    //                ResponseDescription = responseData.ResponseDescription,
    //                CustomerMessage = responseData.CustomerMessage,
    //            };
    //        }
    //        else
    //        {
    //            // Return a failure response if the API call fails
    //            return new STKPushResponse
    //            {
    //                MerchantRequestID = "",
    //                CheckoutRequestID = "",
    //                ResponseCode = responseData?.ResponseCode ?? "1",
    //                ResponseDescription = responseData?.ResponseDescription ?? "Failed to initiate STK push.",
    //                CustomerMessage = responseData?.CustomerMessage ?? "Failed to initiate STK push.",
    //            };
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Handle the exception by returning a generic error response
    //        return new STKPushResponse
    //        {
    //            MerchantRequestID = "",
    //            CheckoutRequestID = "",
    //            ResponseCode = "1",
    //            ResponseDescription = "An error occurred while initiating the STK push.",
    //            CustomerMessage = "An error occurred while initiating the STK push.",
    //        };
    //    }
    //}

    private async Task<string> GetAccessTokenAsync()
    {
        var client = _clientFactory.CreateClient();
        client.BaseAddress = new Uri("https://sandbox.safaricom.co.ke/");

        string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ConsumerKey}:{ConsumerSecret}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        HttpResponseMessage response = await client.GetAsync("oauth/v1/generate?grant_type=client_credentials");

        if (response.IsSuccessStatusCode)
        {
            string jsonResponse = await response.Content.ReadAsStringAsync();
            dynamic tokenResponse = JsonConvert.DeserializeObject(jsonResponse);
            return tokenResponse.access_token;
        }
        else
        {
            throw new Exception($"Failed to get access token. Status: {response.StatusCode}");
        }
    }

    private string GeneratePassword(string timestamp)
    {
        string concatenatedString = $"{BusinessShortCode}{PassKey}{timestamp}";
        byte[] bytes = Encoding.UTF8.GetBytes(concatenatedString);
        return Convert.ToBase64String(bytes);
    }

    public async Task<STKPushResponse> InitiateMpesaSTKPush(PaymentDetailsDto paymentDetails) 
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var generatedPassword = GeneratePassword(timestamp);

        var payload = new
        {
            BusinessShortCode = BusinessShortCode,
            Password = generatedPassword,
            Timestamp = timestamp,
            TransactionType = "CustomerPayBillOnline",
            Amount = 1,
            PartyA = paymentDetails.PaymentReference,
            PartyB = BusinessShortCode,
            PhoneNumber = paymentDetails.PaymentReference,
            CallBackURL = "https://mydomain.com/path", // Replace with your actual callback URL
            AccountReference = "Test",
            TransactionDesc = "Test"
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);

        try
        {
            string accessToken = await GetAccessTokenAsync();

            using var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri("https://sandbox.safaricom.co.ke/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("mpesa/stkpush/v1/processrequest", content);

            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var stkPushResponse = JsonConvert.DeserializeObject<STKPushResponse>(responseData);
                return stkPushResponse;
            }
            else
            {
                string errorResponse = await response.Content.ReadAsStringAsync();
                throw new Exception($"STK Push failed. Status: {response.StatusCode}, Response: {errorResponse}");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while initiating STK Push: {ex.Message}");
        }
    }


    private string GeneratePassword()
    {
        var shortcode = "174379"; // Replace with your shortcode
        //var passkey = "bfb279f9aa9bdbcf158e97dd71a467cd2f54f2a74b1cfcfc9e68d8f7cbe72956"; // Replace with your passkey
        var passkey = _mpesaSandBox.Passkey;
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

        //concatenate string
        var concatenatedString = $"{shortcode}{passkey}{timestamp}";
        byte[] bytes = Encoding.UTF8.GetBytes(concatenatedString);

        return Convert.ToBase64String(bytes);
    }

    //PUBLISH TO ORDEREVENT
    //public async Task PublishOrderEvent(Order order)
    //{
    //    // Deserialize the ProductsJson into a List<Product>
    //    var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductDto>>(order.ProductsJson);

    //    // Create an OrderEvent object
    //    var orderEvent = new OrderEvent
    //    {
    //        OrderID = order.OrderID,
    //        OrderDate = order.OrderDate,
    //        MerchantName = "MinimartKe",
    //        UserID = order.UserID,
    //        products = products, // Assign the deserialized products
    //        UserEmail = "user@gmail.com",
    //        MerchantEmail = "merchant@gmail.com",
    //        //UserPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
    //        UserPhoneNumber = "254794129559",
    //        //MerchantPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
    //        MerchantPhoneNumber = "254794129559",
    //        addresses = order.ShippingAddress,
    //        Amount = order.TotalPaymentAmount
    //    };

    //    // Publish the order event
    //    await _orderEventPublisher.PublishOrderEvent(orderEvent);
    //}

    public async Task PublishOrderEvent(Orders order)
    {
        // Deserialize the ProductsJson into a List<ProductDto>
        var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductDto>>(order.ProductsJson);

        // Group products by MerchantId
        var productsByMerchant = products.GroupBy(p => p.merchantId);

        // Loop through each merchant's products
        foreach (var merchantGroup in productsByMerchant)
        {
            var merchantId = Convert.ToInt16(merchantGroup.Key);

            // Fetch merchant details dynamically (e.g., from a database or service)
            var merchant = await _systemMerchants.GetMerchantByIdAsync(merchantId); // Assuming you have a service to fetch merchant details

            if (merchant == null)
            {
                // Handle case where merchant is not found
                continue;
            }

            // Create an OrderEvent object for this merchant
            var orderEvent = new OrderEvent
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                MerchantName = merchant.MerchantName, // Dynamically loaded merchant name
                UserID = order.UserID,
                products = merchantGroup.ToList(), // Assign products for this merchant
                UserEmail = "user@gmail.com", // Replace with dynamic user email if available
                MerchantEmail = merchant.Email, // Dynamically loaded merchant email
                UserPhoneNumber = "254794129559", // Replace with dynamic user phone number if available
                MerchantPhoneNumber = merchant.Phone, // Dynamically loaded merchant phone number
                addresses = order.ShippingAddress,
                Amount = merchantGroup.Sum(p => p.Price * p.Quantity) // Calculate total amount for this merchant's products
            };

            // Publish the order event for this merchant
            await _orderEventPublisher.PublishOrderEvent(orderEvent);
        }
    }



}
