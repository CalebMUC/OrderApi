using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.DTOS;
using Minimart_Api.TempModels;
using System.Text.RegularExpressions;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Authentication_and_Authorization_Api.Core;
using Microsoft.AspNetCore.SignalR;
using Minimart_Api.Services.SignalR;

namespace Minimart_Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MinimartDBContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly CoreLibraries _coreLibraries;
        private readonly IHubContext<ActivityHub> _hubContext;
        public AuthRepository(MinimartDBContext dbContext, IOptions<JwtSettings> jwtsettings, CoreLibraries coreLibraries,
            IHubContext<ActivityHub> hubContext)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtsettings.Value;
            _coreLibraries = coreLibraries;
            _hubContext = hubContext;

        }

        public async Task<RegisterResponse> Register(Register register)
        {   //an array of bytes
            byte[] salt;
            try
            {
                //check if user already exists by email
                var existingemail = await _dbContext.TUsers.FirstOrDefaultAsync(e => e.Email == register.Email);
                if (existingemail != null) {

                    return new RegisterResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided Email Already Exists"
                    };
                }
                var existingphone = await _dbContext.TUsers.FirstOrDefaultAsync(e => e.PhoneNumber == register.PhoneNumber);
                if (existingphone != null) {

                    return new RegisterResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided phonenumber Already Exists"
                    };

                }

                var hashedPassword = HashPassword(register.password, out salt);

                //create an instamce of userInfo
                var user = new TUser
                {

                    UserName = register.UserName,
                    Password = hashedPassword,
                    Email = register.Email,
                    PhoneNumber = register.PhoneNumber,
                    RoleId = "User",
                     IsAdmin= false,
                    CreatedAt = DateTime.UtcNow,
                    IsLoggedIn= false,
                    LastLogin = null,
                    PasswordChangesOn = null,
                    FailedAttempts = 0,
                    Salt = Convert.ToBase64String(salt),


                };


                _dbContext.TUsers.Add(user);

                await _dbContext.SaveChangesAsync();

                var UserID = user.UserId;

                _hubContext.Clients.All.SendAsync("ReceiveNewUser", $"New User has Registered : {register.UserName} UserID : {UserID}");



                return new RegisterResponse
                {
                    ResponseCode = 200,
                    ResponseMessage = "You have successfully created an account. Welcome to Minimart!",
                    UserID = UserID,
                    Username = register.UserName

                };

            }
            catch (Exception ex) {

                return new RegisterResponse
                {
                    ResponseCode = 500,
                    ResponseMessage = ex.Message,
                };

            }

        }

        public async Task<LoginResponse> Login(UserLogin userLogin)
        {
            if (string.IsNullOrEmpty(userLogin.EmailorPhone) || string.IsNullOrEmpty(userLogin.Password))
            {
                return new LoginResponse
                {
                    ResponseCode = 400,
                    ResponseMessage = "Email/Phone and Password are required"
                };
            }

            string token = "";
            var refreshToken = string.Empty;

            try
            {
                // Validate input format
                var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                var phoneRegex = @"^\+?[0-9]{10,15}$";

                bool isEmail = Regex.IsMatch(userLogin.EmailorPhone, emailRegex);
                bool isPhonenumber = Regex.IsMatch(userLogin.EmailorPhone, phoneRegex);

                if (!isEmail && !isPhonenumber)
                {
                    return new LoginResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Invalid email or phone number format"
                    };
                }

                // Fetch user from database
                var user = isEmail
                    ? await _dbContext.TUsers.FirstOrDefaultAsync(e => e.Email == userLogin.EmailorPhone)
                    : await _dbContext.TUsers.FirstOrDefaultAsync(e => e.PhoneNumber == userLogin.EmailorPhone);

                if (user == null)
                {
                    return new LoginResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = isEmail
                            ? "User with the provided email doesn't exist"
                            : "User with the provided phone number doesn't exist"
                    };
                }

                // Verify password
                string storedHash = user.Password;
                byte[] storedSalt = Convert.FromBase64String(user.Salt);

                bool isPasswordValid = VerifyPassword(userLogin.Password, storedHash, storedSalt);
                if (!isPasswordValid)
                {
                    return new LoginResponse
                    {
                        ResponseCode = 401,
                        ResponseMessage = "Invalid Password"
                    };
                }

                // Generate Token and Refresh Token
                token = _coreLibraries.GenerateToken(user);
                refreshToken = GenerateRefreshToken();

                // Update user details
                user.IsLoggedIn = true;
                user.LastLogin = DateTime.Now;
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                _dbContext.TUsers.Update(user);
                await _dbContext.SaveChangesAsync();

                return new LoginResponse
                {
                    Username = user.UserName,
                    UserID = user.UserId,
                    UserRole = user.RoleId,
                    Token = token,
                    Refreshtoken = refreshToken,
                    ResponseCode = 200,
                    ResponseMessage = "Login Successful"
                };
            }
            catch (DbUpdateException)
            {
                return new LoginResponse
                {
                    ResponseCode = 500,
                    ResponseMessage = "Database error occurred during login"
                };
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return new LoginResponse
                {
                    ResponseCode = 500,
                    ResponseMessage = "An unexpected error occurred"
                };
            }
        }

        public string GenerateRefreshToken() {
            using (var rngCryptoServiceProvide = new RNGCryptoServiceProvider()) { 
                var randomBytes = new byte[32];
                rngCryptoServiceProvide.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public string GenerateJwtToken(TUser user)
        {
            //var jwtSettings = _configuration.GetSection("JwtSettings")/*;*/

            // Get the key, issuer, audience, and expiration time from configuration
            var secretKey = _jwtSettings.Secret;
            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var expirationInMinutes = _jwtSettings.ExpirationInMinutes;

            // Create security key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create claims for the token (like UserId, Role, etc.)
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID for the token
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // User ID claim
            new Claim(ClaimTypes.Role, user.RoleId.ToString() )// Role claim
        };

            // Create the JWT token
            var token = new JwtSecurityToken(
                // issuer: issuer,
                // audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expirationInMinutes),
                signingCredentials: credentials);

            // Serialize the token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string enteredPassword,string storedHash,byte[] storedSalt)
        {
            var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: storedSalt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8

                ));

            return hashed == storedHash;

        }

        public string HashPassword(string password, out byte[] salt) {

            salt = new byte[128 / 8];

            try
            {
                //Generate a random Number and get bytes
                using ( var rng = RandomNumberGenerator.Create()) { 
                    rng.GetBytes(salt);
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password  : password,
                    salt : salt,
                    prf : KeyDerivationPrf.HMACSHA256,
                    iterationCount : 10000,
                    numBytesRequested: 256/8
                    ));

                return hashed;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }

        }

    }
}
