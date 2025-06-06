using Authentication_and_Authorization_Api.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Win32;
using Minimart_Api.Models;
using Minimart_Api.Mappings;
using Minimart_Api.Services;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Minimart_Api.Mappings;
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.DTOS.Products;
using Minimart_Api.DTOS.Address;
using Minimart_Api.DTOS.Cart;
using Minimart_Api.DTOS.Notification;
using Minimart_Api.DTOS.Category;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;

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



        //[HttpGet("GetDashBoardCategories")]
        //public async Task<IActionResult> GetDashBoardCategories()
        //{
        //    //var jsonSrting = JsonConvert.SerializeObject(dashBoardName);

        //    try
        //    {
        //        var Response = await _myService.GetDashBoardCategories();

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}

     


        //[HttpPost("GetProductFeatures")]
        //public async Task<IActionResult> GetProductFeatures([FromBody] AddToCart cartitems)
        //{
        //    var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        var Response = await _myService.AddToCart(jsonSrting);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}

        


        //[HttpPost("GetSearchItem")]
        //public async Task<IActionResult> GetSearchItem([FromBody] AddToCart cartitems)
        //{
        //    var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        var Response = await _myService.AddToCart(jsonSrting);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}


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

      

       


        //[AllowAnonymous]
        //[HttpPost("Login")]
        //public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        //{
        //    // Serialize User Credentials
        //    var jsonDataUserCredentials = JsonConvert.SerializeObject(userLogin);

        //    try
        //    {
        //        // Call the service to authenticate user
        //        var response = await _myService.Login(jsonDataUserCredentials);

        //        if ( Convert.ToBoolean(response.Status.ResponseCode))
        //        {
        //            // Generate access and refresh tokens
        //            var token = _coreLibraries.GenerateToken(response);
        //            var refreshToken = CoreLibraries.GenerateRefreshToken(response.Name);

        //            // Save refresh token
        //            _myService.SaveRefreshToken(JsonConvert.SerializeObject(refreshToken));

        //            // Return success response
        //            return Ok(new
        //            {
        //                responseCode = true,
        //                responseMessage = "Login successful",
        //                accessToken = token,
        //                refreshToken = refreshToken.RefreshToken,
        //                userID = response.UserInfoId,
        //                userName = response.Name,
        //                roleID = response.RoleID
        //            });
        //        }

        //        // Invalid credentials or user not found
        //        return NotFound(new
        //        {
        //            responseCode = false,
        //            responseMessage = response.Status.ResponseMessage
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle unexpected errors
        //        return BadRequest(new
        //        {
        //            responseCode = false,
        //            responseMessage = "An error occurred during login.",
        //            exceptionMessage = ex.Message
        //        });
        //    }
        //}



        //[AllowAnonymous]
        //[HttpPost("Register")]
        //public async Task<IActionResult> Register([FromBody] Register register)
        //{
        //    string userName = null;
        //    string phoneNumber = null;
        //    string Email = null;
        //    string password = null;
        //    string reEnteredPassword = null;
        //    string message = null;


        //    try
        //    {
        //        userName = register.UserName;
        //        phoneNumber = register.PhoneNumber;
        //        Email = register.Email;
        //        password = register.password;
        //        reEnteredPassword = register.ReEnteredpassword;

        //       // myConnectionString = _config.GetConnectionString("myConnectionString");


        //        //message = "You can successfuly created an Account with MiniMart, you can proceed shopping with us";

        //        //logRequests
        //        //logs.LogRequest($"UserName : {userName}, phoneNumber: {phoneNumber}, password: {password}, reEnteredPassword : {reEnteredPassword}", "registerlogs.txt");


        //        // serialize json data

        //        string jsodata = JsonConvert.SerializeObject(register);

        //        //logs.LogRequest(jsodata, "registerlogs.txt");





        //        // pass data for processing

        //        var response = await  _myService.UserRegistration(jsodata);




        //            var jsonResponse = new
        //        {
        //            message = response.ResponseMessage,
        //            username = userName,
        //            userID = response.UserID,
        //            responseCode = response.ResponseCode

        //        };

        //        return new OkObjectResult(jsonResponse);




        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex);
        //    }

        //}

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
                UserInfo response = await _myService.GetRefreshToken(UserID);

                //return response;

                if (Convert.ToBoolean(response.Status.ResponseCode))//--true
                {
                    //Generate a new Json Web Token

                    //UserInfo usrInfo = new UserInfo
                    //{
                    //    Name = refreshTokenRequest.UserID,

                    //    Password = response.Email,
                    //    Email = "muchiricaleb05@gmail.com",
                    //    Role = "Adminstrator"
                    //};

                    //var token = _coreLibraries.GenerateToken(response);
                    var token = "";

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




        //[HttpPost("AddToCart")]
        //public async Task<IActionResult> CheckOut([FromBody] AddToCart cartitems)
        //{
        //    var jsonSrting = JsonConvert.SerializeObject(cartitems);

        //    try
        //    {
        //        var Response = await _myService.AddToCart(jsonSrting);

        //        return Ok(Response);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }



        //}



        //[HttpPost("UploadImages")]
        //public async Task<IActionResult> UploadImages(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("No file uploaded");
        //    }

        //    var bucketName = "minimartke-products-upload";

        //    var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

        //    using var client = new AmazonS3Client();

        //    using var newMemoryStream = new MemoryStream();
        //    await file.CopyToAsync(newMemoryStream);

        //    var uploaadRquest = new PutObjectRequest { 
        //        InputStream = newMemoryStream,
        //        BucketName = bucketName,
        //        Key = $"product-images/{filename}",
        //        ContentType = file.ContentType,
        //        CannedACL = S3CannedACL.PublicRead
        //    }

        //    await client.PutObjectAsync(uploaadRquest);

        //    var fileUrl = $"https://{bucketName}.s3.amazonaws.com/product-images/{fileName}";

        //    // Define the path to the 'uploads' folder within the 'wwwroot' directory
        //    //var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");


        //    //var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        //    //// Check if directory exists; if not, create it
        //    //if (!Directory.Exists(uploadsDir))
        //    //{
        //    //    Directory.CreateDirectory(uploadsDir);
        //    //}

        //    //// Generate a unique filename to avoid collisions

        //    //var filepath = Path.Combine(uploadsDir, filename);

        //    //// Save the file to the uploads directory
        //    //using (var stream = new FileStream(filepath, FileMode.Create))
        //    //{
        //    //    await file.CopyToAsync(stream);
        //    //}

        //    //// Generate the absolute URL to access the file
        //    //var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{filename}";

        //    return Ok(new { Url = fileUrl });
        //}


        [HttpPost("UploadImages")]
        public async Task<IActionResult> UploadImages(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            try
            {
                // 1. Configure S3 Client with Environment Variables (Production/Development)
                var s3Config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.GetBySystemName(
                        Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1"
                    )
                };

                using var client = new AmazonS3Client(
                    Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID") ?? "AKIARJHKLYVKSAVJHAPL",
                    Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY")  ?? "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",
                    s3Config
                );

                // 2. Bucket Name from Environment (with fallback)
                var bucketName = Environment.GetEnvironmentVariable("AWS_S3_BUCKET") ?? "minimartke-products-upload";
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var folderPrefix = Environment.GetEnvironmentVariable("S3_UPLOAD_FOLDER") ?? "product-images";

                // 3. Upload to S3
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var uploadRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = $"{folderPrefix}/{fileName}",
                    InputStream = memoryStream,
                    ContentType = file.ContentType,
                    CannedACL = S3CannedACL.PublicRead
                };

                await client.PutObjectAsync(uploadRequest);

                // 4. Generate URL (with CloudFront fallback to S3)
                var cdnUrl = Environment.GetEnvironmentVariable("CLOUDFRONT_URL");
                var fileUrl = cdnUrl != null
                    ? $"{cdnUrl}/{folderPrefix}/{fileName}"
                    : $"https://{bucketName}.s3.{s3Config.RegionEndpoint.SystemName}.amazonaws.com/{folderPrefix}/{fileName}";

                return Ok(new { Url = fileUrl });
            }
            catch (AmazonS3Exception ex)
            {
                // Log the error (implement your logging)
                return StatusCode(500, $"S3 Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Fallback to local storage if AWS fails (development only)
                //if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                //{
                //    try
                //    {
                //        var localPath = Path.Combine("wwwroot", "uploads", fileName);
                //        using var stream = new FileStream(localPath, FileMode.Create);
                //        await file.CopyToAsync(stream);
                //        return Ok(new { Url = $"/uploads/{fileName}" });
                //    }
                //    catch
                //    {
                //        return StatusCode(500, "Both S3 and local storage failed");
                //    }
                //}
                return StatusCode(500, "Upload failed");
            }
        }

    }
}
