using Authentication_and_Authorization_Api.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using Minimart_Api.Mappings;
using Minimart_Api.Services;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Minimart_Api.Mappings;

namespace Minimart_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntitiesController : ControllerBase
    {
        private readonly IMyService _myService;

        private readonly IConfiguration _config;

        private readonly CoreLibraries _coreLibraries;

        private readonly OrderMapper _orderMapper;



        
        public EntitiesController(IMyService myService, IConfiguration config, CoreLibraries coreLibraries,OrderMapper oderMapper)
        {
            _myService = myService;
            _config = config;
            
            _coreLibraries = coreLibraries;
            _orderMapper = oderMapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEntities()
        {
            try
            {
                var entities = await _myService.GetEntitiesAsync();

                return Ok(entities);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("UserName")]
        public async Task<IActionResult> GetAsyncUserName([FromQuery] string UserName)
        {
            var entities = await _myService.GetAsyncUserName(UserName);

            return Ok(entities);
        }

        [HttpPost("AddCartItems")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCart cartitems)
        {
            var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.AddToCart(jsonSrting);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPost("GetCartItems")]
        public async Task<IActionResult> GetCartItems([FromBody] GetCartItems cartitems)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.GetCartItems(cartitems.UserID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        //[HttpPost("GetDashBoardName")]
        //public async Task<IActionResult> GetDashBoardName([FromBody] DashBoardName dashBoardName)
        //{
        //    var jsonSrting = JsonConvert.SerializeObject(dashBoardName);

        //    try
        //    {
        //        var Response = await _myService.GetDashBoardName(dashBoardName);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}


        [HttpPost("GetProductsByCategory")]
        public async Task<IActionResult> GetProductsByCategory([FromBody] SubCategory categoryName)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.GetProductsByCategory(categoryName.CategoryID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpGet("GetDashBoardCategories")]
        public async Task<IActionResult> GetDashBoardCategories()
        {
            //var jsonSrting = JsonConvert.SerializeObject(dashBoardName);

            try
            {
                var Response = await _myService.GetDashBoardCategories();

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("SubCategoryID")]
        public async Task<IActionResult> SubCategoryID([FromBody] SubCategory categoryName)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.GetSubCategory(categoryName.CategoryName);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }


        [HttpPost("GetProductFeatures")]
        public async Task<IActionResult> GetProductFeatures([FromBody] AddToCart cartitems)
        {
            var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.AddToCart(jsonSrting);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("LoadProductImages")]
        public async Task<IActionResult> LoadProductImages([FromBody] ProductDetails productDetails)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.LoadProductImages(productDetails.ProductID);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }


        [HttpPost("GetSearchItem")]
        public async Task<IActionResult> GetSearchItem([FromBody] AddToCart cartitems)
        {
            var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.AddToCart(jsonSrting);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("FetchAllProducts")]
        //public async Task<IActionResult> LoadMainImages()
        public async Task<IActionResult> FetchAllProducts()
        {
            

            try
            {
                var Response = await _myService.FetchAllProducts();

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        [HttpPost("AddProducts")]
        public async Task<IActionResult> AddProducts([FromBody] AddProducts products)
        {
            //var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.AddProducts(products);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }

        //[HttpPost("Orders")]
        //public async Task<IActionResult> Orders([FromBody] OrderDTO orders)
        //{
        //    //var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        //var Response = await _myService.AddProducts(products);

        //        var order = _orderMapper.MapToEntity(orders);

        //        var response = _myService.CreateOrder(order);

        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}

        //[HttpPost("GetOrders")]
        //public async Task<IActionResult> GetOrders([FromBody] string orderID)
        //{
        //    //var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        var order = await _myService.GetOrderByIdAsync(orderID); // Fetch from service/repo

        //        if (order == null)

        //        {
        //            return NotFound();
        //        }

        //        // Map the Order entity to OrderDto
        //        var orderDto = _orderMapper.MapToDto(order);

        //        return Ok(orderDto);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}

        [HttpGet("counties")]
        public async Task<IActionResult> GetCounties()
        {
            var counties = await _myService.GetCountiesAsync();
            return Ok(counties);
        }

        [HttpGet("towns")]
        public async Task<IActionResult> GetTowns(int countyId)
        {
            var towns = await _myService.GetTownsByCountyAsync(countyId);
            return Ok(towns);
        }

        [HttpGet("deliveryStations")]
        public async Task<IActionResult> GetDeliveryStations(int townId)
        {
            var deliveryStations = await _myService.GetDeliveryStationsByTownAsync(townId);
            return Ok(deliveryStations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAddressById(int id)
        {
            var address = await _myService.GetAddressByIdAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            return Ok(address);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAddressesByUserId(int userId)
        {
            var addresses = await _myService.GetAddressesByUserIdAsync(userId);
            return Ok(addresses);
        }

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress([FromBody] AddressDTO address)
        {
            await _myService.AddAddressAsync(address);
            return CreatedAtAction(nameof(GetAddressesByUserId), new { userId = address.UserID }, address);

        }
        [HttpPost("EditAddress")]
        public async Task<IActionResult> EditAddress([FromBody] EditAddressDTO address)
        {
            try
            {
                await _myService.EditAddressAsync(address);
                return CreatedAtAction(nameof(GetAddressesByUserId), new { userId = address.UserID }, address);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        //[AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            //to log incoming requests
            //logs.LogRequest($" UserName:{userLogin.UserName}, Password : {userLogin.Password}", "logRequest.txt");

           // myConnectionString = _config.GetConnectionString("myConnectionString");

            //serialize User Credentials

            var jsonDataUserCredentials = JsonConvert.SerializeObject(userLogin);
           

            try
            {
                UserInfo Response = await _myService.Login(jsonDataUserCredentials);

                if (Response.StatusId == 1)
                {
                    var token = _coreLibraries.GenerateToken(Response);

                    var refreshToken = CoreLibraries.GenerateRefreshToken(Response.Name);

                    var UserID = Response.UserInfoId;

                    var UserName = Response.Name;


                    //Save RefreshToken to dataBase



                    //Serialize RefreshToken

                    var jsonData = JsonConvert.SerializeObject(refreshToken);

                    _myService.SaveRefreshToken(jsonData);


                    //Set Refresh Token

                    CoreLibraries.SetRefreshToken(refreshToken);



                    return Ok(new
                    {
                        accessToken = token,
                        refreshToken = refreshToken.RefreshToken,
                        userID = UserID,
                        UserName = UserName,

                    });

                    //Generate a RefreshToken


                }

                return NotFound("Invalid Credentials");


            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            string userName = null;
            string phoneNumber = null;
            string Email = null;
            string password = null;
            string reEnteredPassword = null;
            string message = null;


            try
            {
                userName = register.UserName;
                phoneNumber = register.PhoneNumber;
                Email = register.Email;
                password = register.password;
                reEnteredPassword = register.ReEnteredpassword;

               // myConnectionString = _config.GetConnectionString("myConnectionString");


                //message = "You can successfuly created an Account with MiniMart, you can proceed shopping with us";

                //logRequests
                //logs.LogRequest($"UserName : {userName}, phoneNumber: {phoneNumber}, password: {password}, reEnteredPassword : {reEnteredPassword}", "registerlogs.txt");


                // serialize json data

                string jsodata = JsonConvert.SerializeObject(register);

                //logs.LogRequest(jsodata, "registerlogs.txt");





                // pass data for processing

                var response = await  _myService.UserRegistration(jsodata);




                    var jsonResponse = new
                {
                    message = response.ResponseMessage,
                    username = userName,
                    userID = response.UserID,
                    responseCode = response.ResponseCode

                };

                return new OkObjectResult(jsonResponse);




            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var newToken = "";
            var refreshToken = Request.Cookies["refereshToken"];

            if (refreshTokenRequest.UserID == null)
            {
                return Ok("UseID cannot be null");
            }

            var UserID = refreshTokenRequest.UserID;

            RefreshTokens refreshTokens = new RefreshTokens()
            {
                RefreshToken = refreshToken,


                UserName = UserID
            };

            //refreshTokens.RefreshToken = refreshToken;



            //myConnectionString = _config.GetConnectionString("myConnectionString");

            //(new System.Linq.SystemCore_EnumerableDebugView<System.Collections.Generic.KeyValuePair<string, string>>(Request.Cookies).Items[0]).Value

            var jsonData = JsonConvert.SerializeObject(refreshTokens);

            bool responseStatusCode = false;

            try
            {
                UserInfo response = await _myService.GetRefreshToken(jsonData);

                //return response;

                if (response.Status.ResponseCode)//--true
                {
                    //Generate a new Json Web Token

                    //UserInfo usrInfo = new UserInfo
                    //{
                    //    Name = refreshTokenRequest.UserID,

                    //    Password = response.Email,
                    //    Email = "muchiricaleb05@gmail.com",
                    //    Role = "Adminstrator"
                    //};

                    var token = _coreLibraries.GenerateToken(response);

                    newToken = token;

                    //Generate a new RefreshToken

                    var newrefreshToken = CoreLibraries.GenerateRefreshToken(UserID);

                    //Save RefreshToken
                    //Serialize RefreshToken

                    var jsonRefreshData = JsonConvert.SerializeObject(newrefreshToken);


                    _myService.SaveRefreshToken(jsonRefreshData);


                    //Set RefreshToken

                    //Set Refresh Token

                    var Cookie = CoreLibraries.SetRefreshToken(newrefreshToken);

                    Response.Cookies.Append("refereshToken", newrefreshToken.RefreshToken, Cookie);





                    return Ok(newToken);



                }
                else
                {
                    return Ok(response.Status.ResponseMessage);
                }




            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }


        }

       


        [HttpPost("AddToCart")]
        public async Task<IActionResult> CheckOut([FromBody] AddToCart cartitems)
        {
            var jsonSrting = JsonConvert.SerializeObject(cartitems);

            try
            {
                var Response = await _myService.AddToCart(jsonSrting);

                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }



        }
        [HttpPost("UploadImages")]
        public async Task<IActionResult> UploadImages(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("No file uploaded");
            }

            // Define the path to the 'uploads' folder within the 'wwwroot' directory
            //var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            // Check if directory exists; if not, create it
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            // Generate a unique filename to avoid collisions
            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filepath = Path.Combine(uploadsDir, filename);

            // Save the file to the uploads directory
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate the absolute URL to access the file
            var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{filename}";

            return Ok(new { Url = fileUrl });
        }

    }
}
