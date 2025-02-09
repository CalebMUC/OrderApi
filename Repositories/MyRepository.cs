using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minimart_Api.DTOS;
using Minimart_Api.Services.RabbitMQ;
using Minimart_Api.TempModels;
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

        public MyRepository(MinimartDBContext myDBContext,IConfiguration configuration,IOptions<MpesaSandBox> mpesaSandBox, IOrderEventPublisher orderEventPublisher)
        {
            _dbContext = myDBContext;
            _configuration = configuration;
            _mpesaSandBox = mpesaSandBox.Value;
            _orderEventPublisher = orderEventPublisher;
        }

        public async Task<IEnumerable<TUser>> GetAllAsync()
        {
            return await _dbContext.TUsers.ToListAsync();
        }

        public async Task<IEnumerable<TUser>> GetAsyncUserName(string userName)
        {
            return await _dbContext.TUsers.Where(u => u.UserName == userName).ToListAsync();
        }

        public async Task<Status> AddToCart(string cartItems)
        {
            var parameters = new[]
            {
                new SqlParameter("@CartItems", cartItems)
            };

            var response = await _dbContext.Statuses.FromSqlRaw("EXEC p_AddToCart @CartItems", parameters).ToListAsync();
            return response.FirstOrDefault();
        }

        public async Task<Status> DeleteCartItems(CartItemsDTO cartItems)
        {
            try {

                var item =  _dbContext.CartItems.FirstOrDefault(c =>
                c.CartItemId == cartItems.CartItemID &&
                c.CartId == cartItems.CartID &&
                c.ProductId == cartItems.ProductID);

                //check if the item Exists
                if (item != null) {
                    _dbContext.CartItems.Remove(item);
                   _dbContext.SaveChanges();
                }

                var response = new Status {
                    ResponseCode = 200,
                    ResponseMessage = "Item Removed Successfully"
                };

                return response;


            }
            catch (Exception ex) {
                var response = new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = ex.Message
                };

                return response;
            }

         
        }

        public async Task<Status> SaveItems(SaveItemsDTO saveItems)
        {
            try
            {
                var item = _dbContext.TProducts.FirstOrDefault(c =>
                    c.ProductId == saveItems.ProductID);

                // Check if the item exists
                if (item == null)
                {
                    return new Status
                    {
                        ResponseCode = 404,
                        ResponseMessage = "Item not found"
                    };
                }

                // Handle null properties
                if (item.IsSaved == null)
                {
                    item.IsSaved = 0; // Set default value
                }
                else
                {
                    item.IsSaved = 1; // Update value
                    //Exclude RowID Explicily to prevent it from beind update
                    _dbContext.Entry(item).Property(x => x.RowId).IsModified = false;
                    await _dbContext.SaveChangesAsync();
                }

                //_dbContext.TProducts.Update(item);
                //await _dbContext.SaveChangesAsync();

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Item Saved Successfully"
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

        public async Task<UserRegStatus> UserRegistration(string jsonData)
        {
            var parameters = new[]
            {
                new SqlParameter("@JsonData", jsonData)
            };

            var response = await _dbContext.UsrRegStatuses.FromSqlRaw("EXEC p_AddUser @JsonData", parameters).ToListAsync();
            return response.FirstOrDefault();
        }

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

        public async Task<IEnumerable<CategoryDTO>> GetDashBoardCategories()
        {
            return await _dbContext.TCategories
                .Include(tc => tc.TSubcategoryids)
                .Select(tc => new CategoryDTO
                {
                    Id = tc.CategoryId,
                    Name = tc.CategoryName,
                    Description = tc.Description,
                    Subcategoryids = tc.TSubcategoryids
                        .Select(sc => new SubCategoryDTO
                        {
                            Id = sc.SubCategoryId,
                            Name = sc.ProductName,
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName dashboardName)
        {
            return await _dbContext.TSubcategoryids.Where(p => p.ProductName == dashboardName.Name).ToListAsync();
        }

        public async Task<IEnumerable<CartResults>> GetSubCategory(string categoryName)
        {
            return await _dbContext.TProducts
                .Where(tp => tp.SubCategoryId == categoryName)
                .Select(tp => new CartResults
                {
                    ProductName = tp.ProductName,
                    ProductImage = tp.ImageUrl,
                    Instock = tp.InStock,
                    price = tp.Price,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CartResults>> GetProductsByCategory(int categoryId)
        {
            return await _dbContext.TProducts
                .Where(tp => tp.CategoryId == categoryId)
                .Select(tp => new CartResults
                {
                    productID = tp.ProductId,
                    ProductName = tp.ProductName,
                    ProductImage = tp.ImageUrl,
                    Instock = tp.InStock,
                    price = tp.Price,
                })
                .ToListAsync();
        }

        public async Task<UserInfo> GetRefreshToken(string jsonData)
        {
            var parameters = new[]
            {
                new SqlParameter("@JsonData", jsonData)
            };

            var response = await _dbContext.Users.FromSqlRaw("EXEC p_AddToCart @JsonData", parameters).ToListAsync();
            return response.FirstOrDefault();
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int userId)
        {
            return await _dbContext.CartItems.Where(ci => ci.Cart.UserId == userId)
                .Select(ci => new CartResults
                {
                    productID = ci.Product.ProductId,
                    ProductImage = ci.Product.ImageUrl,
                    ProductName = ci.Product.ProductName,
                    Quantity = ci.Quantity,
                    price = ci.Product.Price,
                    Instock = ci.Product.InStock,
                    ProductDescription = ci.Product.ProductDescription,
                    KeyFeatures = ci.Product.KeyFeatures,
                    Specification = ci.Product.Specification,
                    Box = ci.Product.Box,
                    CartID = ci.CartId ?? 0,
                    CartItemID = ci.CartItemId,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TProduct>> GetSavedItems()
        {
            return await _dbContext.TProducts.Where(P=> P.IsSaved == 1)
                .Select(ci => new TProduct
                {
                    ProductId = ci.ProductId,
                    ImageUrl = ci.ImageUrl,
                    ProductName = ci.ProductName,
                    //Quantity = ci.InStock,
                    Price = ci.Price,
                    InStock = ci.InStock,
                    ProductDescription = ci.ProductDescription,
                    KeyFeatures = ci.KeyFeatures,
                    Specification = ci.Specification,
                    Box = ci.Box
                })
                .ToListAsync();
        }

     

        public async Task<IEnumerable<TProduct>> FetchAllProducts()
        {
            return await _dbContext.TProducts.ToListAsync();
        }

        public async Task<IEnumerable<TProduct>> LoadProductImages(string productId)
        {
            return await _dbContext.TProducts
                .Where(w => w.ProductId == productId.ToString())
                .ToListAsync();
        }
        public async Task<IEnumerable<County>> GetAllCountiesAsync()
        {
            return await _dbContext.Counties.ToListAsync();
        }

        public async Task<IEnumerable<Town>> GetTownsByCountyAsync(int countyId)
        {
            return await _dbContext.Towns.Where(t => t.CountyId == countyId).ToListAsync();
        }

        public async Task<IEnumerable<DeliveryStation>> GetDeliveryStationsByTownAsync(int townId)
        {
            return await _dbContext.DeliveryStations.Where(ds => ds.TownId == townId).ToListAsync();
        }
        public async Task<ResponseStatus> AddOrder(OrderListDto transaction)
        {
            using var transactionScope = await _dbContext.Database.BeginTransactionAsync();

            int paymentMethodID = 0;
            var newOrder = new Order();

            try
            {
                // Iterate through each order in the transaction
                foreach (var orderDto in transaction.Orders)
                {
                    // Check if PaymentID exists in the Payments table
                    foreach (var paymentDetailDto in orderDto.PaymentDetails)
                    {
                        var paymentExists = _dbContext.payments
                            .Any(p => p.PaymentID == paymentDetailDto.PaymentID);

                        if (!paymentExists)
                        {
                            // Handle the case where PaymentID does not exist
                            throw new Exception($"PaymentID {paymentDetailDto.PaymentID} does not exist in the Payments table.");
                        }

                        if (paymentDetailDto.PaymentMethod == "Mpesa")
                        {
                            var stkPushResponse = await InitiateMpesaSTKPush(paymentDetailDto);

                            if (stkPushResponse.ResponseCode == "1" )
                            {
                                return new ResponseStatus
                                {
                                    ResponseStatusId = 400,
                                    ResponseMessage = "M-Pesa STK Push failed: " + stkPushResponse.CustomerMessage
                                };
                            }

                            // Save STK Push details for auditing
                            var newPayment = new PaymentDetails
                            {
                                PaymentID = paymentDetailDto.PaymentID,
                                TrxReference = stkPushResponse.CheckoutRequestID, // Use CheckoutRequestID from the response
                                Amount = paymentDetailDto.Amount,
                                PaymentDate = DateTime.UtcNow,
                                Phonenumber = paymentDetailDto.Phonenumber,
                                
                            };

                            _dbContext.paymentDetails.Add(newPayment);
                            await _dbContext.SaveChangesAsync();

                            // Fetch the PaymentMethodID for use in the order
                            paymentMethodID = newPayment.PaymentMethodID;
                        }

                        // Proceed with the order creation
                        newOrder = new Order
                        {
                            OrderID = orderDto.OrderID,
                            UserID = orderDto.UserID,
                            OrderDate = orderDto.OrderDate,
                            DeliveryScheduleDate = orderDto.DeliveryScheduleDate,
                            OrderedBy = orderDto.OrderedBy,
                            Status = orderDto.Status,
                            PaymentMethodID = paymentMethodID, // Assign the correct PaymentMethodID here
                            PaymentConfirmation = orderDto.PaymentConfirmation,
                            TotalOrderAmount = orderDto.TotalOrderAmount,
                            TotalPaymentAmount = orderDto.TotalPaymentAmount,
                            TotalDeliveryFees = orderDto.TotalDeliveryFees,
                            TotalTax = 0,

                            // Map PaymentDetails JSON
                            PaymentDetailsJson = JsonConvert.SerializeObject(new
                            {
                                PaymentID = paymentDetailDto.PaymentID,
                                PaymentReference = paymentDetailDto.PaymentReference,
                                Amount = paymentDetailDto.Amount,
                                PaymentDate = DateTime.UtcNow // Use the current date
                            }),

                            // Map Products JSON
                            ProductsJson = JsonConvert.SerializeObject(orderDto.Products.Select(p => new
                            {
                                ProductName = p.ProductName,
                                ProductID = p.ProductID,
                                Quantity = p.Quantity,
                                Price = p.Price,
                                Discount = p.Discount
                            }).ToList()),

                            // Map the collection of OrderProducts
                            OrderProducts = orderDto.Products.Select(productDto => new OrderProducts
                            {
                                ProductID = productDto.ProductID,
                                Quantity = productDto.Quantity,
                            }).ToList(),

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

                        // Update product quantities in stock
                        foreach (var product in orderDto.Products)
                        {
                            var existingProduct = await _dbContext.TProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
                            if (existingProduct != null)
                            {
                                // Reduce product quantity
                                existingProduct.InStock -= product.Quantity;

                                // Prevent negative quantity
                                if (existingProduct.InStock < 0)
                                {
                                    return new ResponseStatus
                                    {
                                        ResponseStatusId = 400,
                                        ResponseMessage = $"Insufficient stock for product: {existingProduct.ProductName}"
                                    };
                                }

                                // Detach the entity from EF tracking to prevent EF from trying to update the RowID
                                _dbContext.Entry(existingProduct).State = EntityState.Detached;

                                // Attach entity and explicitly set the properties to update
                                _dbContext.TProducts.Attach(existingProduct);
                                _dbContext.Entry(existingProduct).Property(x => x.InStock).IsModified = true;
                            }
                        }



                        // Add the new order to the context
                        _dbContext.orders.Add(newOrder);
                    }
                }

                // Save all changes to the database
                await _dbContext.SaveChangesAsync();
                await transactionScope.CommitAsync();

               //await  PublishOrderEvent(newOrder);

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


        public async Task<Address> GetAddressByIdAsync(int addressId)
        {
            return await _dbContext.Addresses.FindAsync(addressId);
        }

        public async Task<IEnumerable<GetAddressDTO>> GetAddressesByUserIdAsync(int userId)
        {
            var addresses = await _dbContext.Addresses
                                             .Where(a => a.UserID == userId)
                                             .ToListAsync();

            // List to hold the addresses with the correct County and Town IDs
            var addressDTOs = new List<GetAddressDTO>();

            foreach (var address in addresses)
            {
                // Fetch the County ID based on the county name
                var county = await _dbContext.Counties
                                             .FirstOrDefaultAsync(c => c.CountyName == address.County);
                // Fetch the Town ID based on the town name
                var town = await _dbContext.Towns
                                           .FirstOrDefaultAsync(t => t.TownName == address.Town);

                // Check if both county and town were found
                if (county != null && town != null)
               
                {
                    // Create an AddressDTO or update address with CountyID and TownID
                    addressDTOs.Add(new GetAddressDTO
                    {
                        AddressID = address.AddressID,
                        UserID = address.UserID,
                        Name = address.Name,
                        PhoneNumber = address.Phonenumber,
                        PostalAddress = address.PostalAddress,
                        CountyId = county.CountyId, // Use CountyID
                        TownId = town.TownId,       // Use TownID
                        PostalCode = address.PostalCode,
                        County = county.CountyName,
                        Town = county.CountyName,
                        ExtraInformation = address.ExtraInformation,
                        isDefault = address.isDefault,
                    });
                }
            }

            return addressDTOs;
        }


        public async Task AddAddressAsync(AddressDTO address)
        {
            var NewAddress = new Address
            {
                UserID = address.UserID,
                Name = address.Name,
                Phonenumber = address.Phonenumber,
                PostalAddress = address.PostalAddress,
                County = address.County,
                Town = address.Town,
                PostalCode = address.PostalCode,
                ExtraInformation = address.ExtraInformation,
                isDefault = address.isDefault,
            };
            await _dbContext.Addresses.AddAsync(NewAddress);
        }
        public async Task EditAddressAsync(EditAddressDTO address)
        {
            // Find the existing address by ID
            var existingAddress = await _dbContext.Addresses.FirstOrDefaultAsync(a => a.AddressID == address.AddressID);

            if (address.isDefault == 1)
            {
                // Find and reset any previous default address for the user
                var defaultAddress = await _dbContext.Addresses
                    .FirstOrDefaultAsync(a => a.UserID == address.UserID && a.isDefault == 1 && a.AddressID != address.AddressID);

                if (defaultAddress != null)
                {
                    defaultAddress.isDefault = 0;
                    _dbContext.Addresses.Update(defaultAddress);
                }
            }

            // Update the existing address fields with new data
            //existingAddress.UserID = address.UserID;
            existingAddress.Name = address.Name;
            existingAddress.Phonenumber = address.Phonenumber;
            existingAddress.PostalAddress = address.PostalAddress;
            existingAddress.County = address.County;
            existingAddress.Town = address.Town;
            existingAddress.PostalCode = address.PostalCode;
            existingAddress.ExtraInformation = address.ExtraInformation;
            existingAddress.isDefault = address.isDefault;

            _dbContext.Addresses.Update(existingAddress);

            // Save the changes to the database
            await _dbContext.SaveChangesAsync();
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        public async Task<ResponseStatus> CreateOrder(Order order)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    // Deserialize PaymentDetails from JSON
                    var newPayment = JsonConvert.DeserializeObject<PaymentDetails>(order.PaymentDetailsJson);

                    if (newPayment != null)
                    {
                        // Ensure the payment date is valid
                        if (newPayment.PaymentDate == DateTime.MinValue)
                        {
                            newPayment.PaymentDate = DateTime.Now;
                        }

                        // Add payment details to the context
                        _dbContext.paymentDetails.Add(newPayment);
                        await _dbContext.SaveChangesAsync();
                    }

                    // Update product stock quantities based on the order
                    foreach (var product in order.OrderProducts)
                    {
                        var existingProduct = await _dbContext.TProducts.FirstOrDefaultAsync(p => p.ProductId == product.ProductID);
                        if (existingProduct != null)
                        {
                            // Check if the stock is sufficient
                            if (existingProduct.InStock < product.Quantity)
                            {
                                return new ResponseStatus
                                {
                                    ResponseStatusId = 400,
                                    ResponseMessage = $"Insufficient stock for product ID: {product.ProductID}"
                                };
                            }

                            existingProduct.InStock -= product.Quantity;
                            _dbContext.TProducts.Update(existingProduct);
                        }
                        else
                        {
                            return new ResponseStatus
                            {
                                ResponseStatusId = 404,
                                ResponseMessage = $"Product ID: {product.ProductID} not found."
                            };
                        }
                    }

                    // Add the order to the context
                    _dbContext.orders.Add(order);
                    await _dbContext.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();

                    return new ResponseStatus
                    {
                        ResponseStatusId = 200,
                        ResponseMessage = "Transaction completed successfully"
                    };
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseStatus
                    {
                        ResponseStatusId = 409,
                        ResponseMessage = $"Concurrency error: {ex.Message}"
                    };
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseStatus
                    {
                        ResponseStatusId = 500,
                        ResponseMessage = $"Database update error: {ex.Message} - Inner: {ex.InnerException?.Message}"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ResponseStatus
                    {
                        ResponseStatusId = 500,
                        ResponseMessage = $"Internal Server Error: {ex.Message} - Inner: {ex.InnerException?.Message}"
                    };
                }
            }
        }




        public async Task<ResponseStatus> AddProducts(AddProducts product)
        {
            
            // Check if the product already exists
            var existingProduct = await _dbContext.TProducts
                .FirstOrDefaultAsync(x => x.ProductName == product.productName);
            if (existingProduct != null)
            {
                return new ResponseStatus
                {
                    ResponseStatusId = 409,
                    ResponseMessage = $"Product '{product.productName}' Already Exists"
                };
            }

            // Create a new product entity
            var newProduct = new TProduct
            {
                ProductId = product.productID,
                ProductName = product.productName,
                ProductDescription = product.productDetails,
                ProductType = "P",
                CategoryId = product.CategoryID,
                SubCategoryId = product.subcategory,
                SearchKeyWord = product.SearchKeyWord,
                Price = product.price,
                InStock = product.Quantity,
                Discount = product.discount,
                ImageUrl = product.productImage,
                KeyFeatures = product.productFeatures,
                Box = product.boxContent,
                Specification = product.productSpecifications,
                CreatedOn = DateTime.Now,
                CreatedBy = product.CreatedBy,
                ImageType = "Image/Jpeg",
               Category = product.Category

            };

            // Add the new product to the database
            _dbContext.TProducts.Add(newProduct);

            try
            {
                // Save changes to the database asynchronously
                await _dbContext.SaveChangesAsync();

                return new ResponseStatus
                {
                    ResponseStatusId = 200,
                    ResponseMessage = "Product Added Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ResponseStatus
                {
                    ResponseStatusId = 500,
                    ResponseMessage = "Internal Server Error: " + ex.Message
                };
            }
        }
    }
}
