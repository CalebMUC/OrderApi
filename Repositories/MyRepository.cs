using Authentication_and_Authorization_Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.Data;
using Minimart_Api.DTOS;


//using Minimart_Api.Data;
using Minimart_Api.Models;

namespace Minimart_Api.Repositories
{
    public class MyRepository : IRepository
    {
        private readonly MinimartDBContext _dbContext;

        //private readonly MinimartDBContext _myDBContext;
        //creates a constructor to get DBClass methods and propertis
        public MyRepository(MinimartDBContext myDBContext)
        {
            _dbContext = myDBContext;

            // _myDBContext = myDBContext;
        }



        public async Task<IEnumerable<TUser>> GetAllAsync()
        {
            return await _dbContext.TUsers.ToListAsync();

        }
        public async Task<IEnumerable<TUser>> GetAsyncUserName(string UserName)
        {
            return await _dbContext.TUsers.Where(u => u.UserName == UserName).ToListAsync();

        }

        public async Task<Status> AddToCart(string CartItems)
        {
            //try
            //{
            //var Response = await _dbContext.Statuses.FromSqlInterpolated($"p_AddToCart{CartItems}").ToListAsync();

            var parameters = new[]
            {
                new SqlParameter("@CartItems", CartItems)

            };



            var Response = await _dbContext.Statuses.FromSqlInterpolated($" EXEC p_AddToCart @CartItems = {CartItems}").ToListAsync();

            return Response.AsEnumerable().FirstOrDefault();
            //}
            //catch (Exception ex) 
            //{
            //return

            //}


        }

        public async Task<UserRegStatus> UserRegistration(string JsonData)
        {
            //try
            //{
            //var Response = await _dbContext.Statuses.FromSqlInterpolated($"p_AddToCart{CartItems}").ToListAsync();

            var parameters = new[]
            {
                new SqlParameter("@JsonData", JsonData)

            };



            var Response = await _dbContext.UsrRegStatuses.FromSqlInterpolated($" EXEC p_AddUser @JsonData = {JsonData}").ToListAsync();

            return Response.AsEnumerable().FirstOrDefault();
            //}
            //catch (Exception ex) 
            //{
            //return

            //}


        }

        public async Task<UserInfo> Login(string jsonData)
        {
            //try
            //{
            //var Response = await _dbContext.Statuses.FromSqlInterpolated($"p_AddToCart{CartItems}").ToListAsync();

            var parameters = new[]
            {
                new SqlParameter("@JsonData", jsonData)

            };



            var Response = await _dbContext.Users.FromSqlInterpolated($" EXEC p_AuthenticateUser @JsonData = {jsonData}").ToListAsync();

            return Response.AsEnumerable().FirstOrDefault();
            //}
            //catch (Exception ex) 
            //{
            //return

            //}


        }

        public void SaveRefreshToken(string jsonData)
        {
            //try
            //{
            //var Response = await _dbContext.Statuses.FromSqlInterpolated($"p_AddToCart{CartItems}").ToListAsync();

            var parameters = new[]
            {
                new SqlParameter("@jsonData", jsonData)

            };



            _dbContext.Database.ExecuteSqlInterpolatedAsync($" EXEC p_SaveRefreshToken @jsonData = {jsonData}");

            //return Response.AsEnumerable().FirstOrDefault();
            //}
            //catch (Exception ex) 
            //{
            //return

            //}


        }
        public async Task<IEnumerable<CategoryDTO>> GetDashBoardCategories()
        {
            var result = await _dbContext.TCategories
                .Include(TC=> TC.TSubcategoryids)
                .Select(TC => new CategoryDTO {
                    Id = TC.CategoryId,
                    Name = TC.CategoryName,
                    Subcategoryids = TC.TSubcategoryids
                    .Select(sc => new SubCategoryDTO { 

                        Id = sc.SubCategoryId,
                        Name = sc.ProductName,
                    })
                    .ToList()
                })
                .ToListAsync();
                ;
           

            return result;
        }

        public async Task<IEnumerable<TSubcategoryid>> GetDashBoardName(DashBoardName Dashboardname)
        {
            var result = await _dbContext.TSubcategoryids.Where(p => p.ProductName == Dashboardname.Name).ToListAsync();


            return result;
        }


        public async Task<IEnumerable<CartResults>> GetSubCategory(string CategoryName)
        {
            //var result = await _dbContext.TSubcategoryids
            //    .Where(p => p.CategoryName == CategoryName)
            //    .Select(p => p.ProductName).ToListAsync();


            var result = await _dbContext.TProducts
                .Where(TP => TP.SubCategoryId == CategoryName )
                .Select(TP =>new CartResults {
                    ProductName = TP.ProductName,
                    ProductImage = TP.ImageUrl,
                    Instock = TP.InStock,
                    price = TP.Price,
                })
                .ToListAsync() ;


            return result;
        }
        public async Task<IEnumerable<CartResults>> GetProductsByCategory(string CategoryID)
        {
            //var result = await _dbContext.TSubcategoryids
            //    .Where(p => p.CategoryName == CategoryName)
            //    .Select(p => p.ProductName).ToListAsync();


            var result = await _dbContext.TProducts
                .Where(TP => TP.Category == CategoryID)
                .Select(TP => new CartResults
                {
                    ProductName = TP.ProductName,
                    ProductImage = TP.ImageUrl,
                    Instock = TP.InStock,
                    price = TP.Price,
                })
                .ToListAsync();


            return result;
        }
        public async Task<UserInfo> GetRefreshToken(string JsonData)
        {

            var parameters = new[]
           {
                new SqlParameter("@CartItems", JsonData)

            };



            var Response = await _dbContext.Users.FromSqlInterpolated($" EXEC p_AddToCart @CartItems = {JsonData}").ToListAsync();



            return Response.AsEnumerable().FirstOrDefault();
        }

        public async Task<IEnumerable<CartResults>> GetCartItems(int UserID)
        {
            var results = await _dbContext.CartItems.Where(ci => ci.Cart.UserId == UserID)
                .Select(ci => new CartResults
                {
                    productID  = ci.Product.ProductId,
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

            return results;
        }
        public async Task<IEnumerable<TProduct>> FetchAllProducts()
        {
            var results = await _dbContext.TProducts
                .Select(p => new TProduct
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ProductDescription = p.ProductDescription,
                    ImageUrl = p.ImageUrl,
                    KeyFeatures = p.KeyFeatures,
                    Specification = p.Specification,
                    Box = p.Box,
                    InStock = p.InStock
                })
                .ToListAsync();

            return results;
        }
        public async Task<IEnumerable<TProduct>> LoadProductImages(int ProductID)
        {
            var result = await _dbContext.TProducts
                .Where(W => W.ProductId == ProductID)
                .Select(p => new TProduct
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    ProductDescription = p.ProductDescription,
                    ImageUrl = p.ImageUrl,
                    KeyFeatures = p.KeyFeatures,
                    Specification = p.Specification,
                    Box = p.Box,
                    InStock = p.InStock
                }).ToListAsync();


            return result;
        }

        
      public async Task<ResponseStatus> AddProducts(AddProducts product)
        {
            // Check if the product already exists
            var existingProduct = _dbContext.TProducts.FirstOrDefault(x => x.ProductName != null && x.ProductName == product.productName);
            if (existingProduct != null)
            {
                return new ResponseStatus
                {
                    ResponseStatusId = 409,
                    ResponseMessage = $"Product '{product.productName}' Already Exists"
                };
            }

            // Create a new product entity
            var newProduct = new TProduct // Assuming your Product entity, not AddProducts DTO, is being used
            {
                ProductName = product.productName,
                ProductDescription = product.productDetails,
                ProductType = product.Category,
                Category = product.Category,
                SubCategoryId = product.subcategory,
                Price = product.price,
                InStock = product.Quantity,
                ImageUrl = product.productImage,
                KeyFeatures = product.productFeatures,
                Box = product.boxContent,
                Specification = product.productSpecifications,
                /*CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow,
                CreatedBy = /* Set current user id or name 
                UpdatedBy = /* Set current user id or name */
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

