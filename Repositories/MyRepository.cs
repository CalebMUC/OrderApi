using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Minimart_Api.Repositories
{
    public class MyRepository : IRepository
    {
        private readonly MinimartDBContext _dbContext;

        public MyRepository(MinimartDBContext myDBContext)
        {
            _dbContext = myDBContext;
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

        public async Task<UserRegStatus> UserRegistration(string jsonData)
        {
            var parameters = new[]
            {
                new SqlParameter("@JsonData", jsonData)
            };

            var response = await _dbContext.UsrRegStatuses.FromSqlRaw("EXEC p_AddUser @JsonData", parameters).ToListAsync();
            return response.FirstOrDefault();
        }

        public async Task<UserInfo> Login(string jsonData)
        {
            var parameters = new[]
            {
                new SqlParameter("@JsonData", jsonData)
            };

            var response = await _dbContext.Users.FromSqlRaw("EXEC p_AuthenticateUser @JsonData", parameters).ToListAsync();
            return response.FirstOrDefault();
        }

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

                        // Insert new PaymentDetails records for audit purposes
                        var newPayment = new PaymentDetails
                        {
                            PaymentID = paymentDetailDto.PaymentID,
                            PaymentReference = paymentDetailDto.PaymentReference,
                            Amount = paymentDetailDto.Amount,
                            PaymentDate = DateTime.UtcNow // Always use the current date for audits
                        };

                        // Add the new payment details record
                        _dbContext.paymentDetails.Add(newPayment);
                        await _dbContext.SaveChangesAsync(); // Save to get the auto-generated PaymentMethodID

                        // Fetch the PaymentMethodID for use in the order
                        var paymentMethodID = newPayment.PaymentMethodID;

                        // Proceed with the order creation
                        var newOrder = new Order
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
                            TotalTax = orderDto.TotalTax,

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
