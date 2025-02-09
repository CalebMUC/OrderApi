using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimart_Api.DTOS;
using Minimart_Api.Repositories;
using Minimart_Api.Services.RabbitMQ;
using Minimart_Api.TempModels;
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

    private readonly IHttpClientFactory _clientFactory;
    private const string ConsumerKey = "vM5KjasAGTVzdddzpP8tENa1Z9us6G6CDjeZzEAHQKzVbQu4";
    private const string ConsumerSecret = "BZQ2uAq84LIzonV6uaXBo7ofYGTHvhhvFD5vVd8EuTwnsd0n0b9ewQ8ExNMKuOnn";
    private const string BusinessShortCode = "174379";
    private const string PassKey = "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
    public OrderRepository(MinimartDBContext dbContext,
        IOrderEventPublisher orderEventPublisher,
        IOptions<MpesaSandBox> mpesaSandBox,
        IHttpClientFactory clientFactory
        /*IConfiguration configuration*/)
    {
        _dbContext = dbContext;
        _orderEventPublisher = orderEventPublisher;
        //_configuration = configuration;
        _mpesaSandBox = mpesaSandBox.Value;
        _clientFactory = clientFactory;

    }

    public async Task<List<GetOrdersDTO>> GetOrdersByStatusAsync(int status, int userID)
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

    //Create Order
    public async Task<ResponseStatus> AddOrder(OrderListDto transaction)
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
                _dbContext.orders.Add(newOrder);
                await _dbContext.SaveChangesAsync();

                await PublishOrderEvent(newOrder);
            }

            await transactionScope.CommitAsync();

            return new ResponseStatus
            {
                ResponseStatusId = 200,
                ResponseMessage = "Transaction completed successfully"
            };
        }
        catch (Exception ex)
        {
            await transactionScope.RollbackAsync();

            return new ResponseStatus
            {
                ResponseStatusId = 500,
                ResponseMessage = $"Internal Server Error: {ex.Message}"
            };
        }
    }

    private async Task<int> HandlePaymentDetails(List<PaymentDetailsDto> paymentDetails)
    {
        int paymentMethodID = 0;

        foreach (var paymentDetailDto in paymentDetails)
        {
            var paymentExists = _dbContext.payments
                .Any(p => p.PaymentID == paymentDetailDto.PaymentID);

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
                    PaymentID = paymentDetailDto.PaymentID,
                    TrxReference = stkPushResponse.CheckoutRequestID,
                    Amount = paymentDetailDto.Amount,
                    PaymentDate = DateTime.UtcNow,
                    PaymentReference = Convert.ToString(paymentDetailDto.Phonenumber)
                };

                _dbContext.paymentDetails.Add(newPayment);
                await _dbContext.SaveChangesAsync();

                paymentMethodID = newPayment.PaymentMethodID;
            }
        }

        return paymentMethodID;
    }

    private async Task<Order> CreateOrder(OrderDTO orderDto, int paymentMethodID)
    {
        return new Order
        {
            OrderID = orderDto.OrderID,
            UserID = orderDto.UserID,
            OrderDate = orderDto.OrderDate,
            DeliveryScheduleDate = orderDto.DeliveryScheduleDate,
            OrderedBy = orderDto.OrderedBy,
            Status = orderDto.Status,
            PaymentMethodID = paymentMethodID,
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
            PickupLocation = JsonConvert.SerializeObject(orderDto.PickUpLocation)
        };
    }
    private async Task UpdateProductStock(List<OrderProductsDTO> products)
    {
        foreach (var product in products)
        {
            var existingProduct = await _dbContext.TProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
            if (existingProduct != null)
            {
                existingProduct.InStock -= product.Quantity;

                if (existingProduct.InStock < 0)
                {
                    throw new Exception($"Insufficient stock for product: {existingProduct.ProductName}");
                }

                //detach entity for EF tracking to prevent updating RowID
                _dbContext.Entry(existingProduct).State = EntityState.Detached;

                //attach and explicitly set properties to update

                _dbContext.TProducts.Attach(existingProduct);

                _dbContext.Entry(existingProduct).Property(x => x.InStock).IsModified = true;
            }
        }
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
    public async Task PublishOrderEvent(Order order)
    {
        // Deserialize the ProductsJson into a List<Product>
        var products = System.Text.Json.JsonSerializer.Deserialize<List<ProductDto>>(order.ProductsJson);

        // Create an OrderEvent object
        var orderEvent = new OrderEvent
        {
            OrderID = order.OrderID,
            OrderDate = order.OrderDate,
            MerchantName = "MinimartKe",
            UserID = order.UserID,
            products = products, // Assign the deserialized products
            UserEmail = "user@gmail.com",
            MerchantEmail = "merchant@gmail.com",
            //UserPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
            UserPhoneNumber = "254794129559",
            //MerchantPhoneNumber = order.PaymentDetails.Phonenumber.ToString(),
            MerchantPhoneNumber = "254794129559",
            addresses = order.ShippingAddress,
            Amount = order.TotalPaymentAmount
        };

        // Publish the order event
        await _orderEventPublisher.PublishOrderEvent(orderEvent);
    }



}
