using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Minimart_Api.Models;
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
using Minimart_Api.DTOS.Authorization;
using Minimart_Api.Data;
using Minimart_Api.Services.EmailServices;
using StackExchange.Redis;
using Minimart_Api.DTOS.General;

namespace Minimart_Api.Repositories.Authorization
{
    public class AuthRepository : IAuthRepository
    {
        private readonly MinimartDBContext _dbContext;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;
        private readonly CoreLibraries _coreLibraries;
        private readonly IHubContext<ActivityHub> _hubContext;
        private readonly ILogger<AuthRepository> _logger;
        private readonly BrevoEmailService _brevoEmailService;
        private readonly IDatabase _redisDatabase;
        public AuthRepository(MinimartDBContext dbContext, IOptions<JwtSettings> jwtsettings,  CoreLibraries coreLibraries,
            IHubContext<ActivityHub> hubContext,ILogger<AuthRepository> logger, BrevoEmailService brevoEmailService, IDatabase redisDatabase )
        {
            _dbContext = dbContext;
            _jwtSettings = jwtsettings.Value;
            _coreLibraries = coreLibraries;
            _hubContext = hubContext;
            _logger = logger;
            _brevoEmailService = brevoEmailService;
            _redisDatabase = redisDatabase;
        }


        public async Task<Status> SendEmailVerificationCode(string email)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided Email Already Exists"
                    };
                }

                // 🔄 Rate Limiting
                string rateLimitKey = $"EmailRateLimit:{email}";
                var attemptCount = await _redisDatabase.StringGetAsync(rateLimitKey);

                if (attemptCount.HasValue && int.Parse(attemptCount) >= 3)
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Too many Password Resets. Please try again Later"
                    };
                }

                // Generate verification code
                var code = new Random().Next(100000, 999999).ToString();
                var verificationKey = $"EmailVerification:{email}";

                // Set verification code in Redis for 10 minutes
                await _redisDatabase.StringSetAsync(verificationKey, code, TimeSpan.FromMinutes(10));

                // Increment rate limit counter (or set it to 1 if not exists)
                if (!attemptCount.HasValue)
                {
                    await _redisDatabase.StringSetAsync(rateLimitKey, "1", TimeSpan.FromMinutes(10));
                }
                else
                {
                    await _redisDatabase.StringIncrementAsync(rateLimitKey);
                }

                await _brevoEmailService.SendAsync(email, "Email Verification Code", $"Your Email Verification Code is {code}");

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Email Validation Code Sent Successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email verification code to {Email}", email);
                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Error Sending Reset Code"
                };
            }
        }

        public async Task<Status> SendResetCode(PasswordResetDto passwordReset)
        {
            try
            {
                //check if user with the provided Email exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == passwordReset.Email);
                if (existingUser == null) {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided Email doesnt Exist"
                    };
                }

                //Rate Limiting Limit the No of trials to 3 
                string RateLimitKey = $"PasswordResetLimit:{passwordReset.Email} ";
                var attemptCount = await _redisDatabase.StringGetAsync(RateLimitKey);

                if (attemptCount.HasValue && int.Parse(attemptCount) >= 3) {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = "Too many Password Resets. Please try again Later"
                    };
                }

                //Generate Password Reset Code
                var code = new Random().Next(100000, 999999).ToString();
                var verificationKey = $"ResetCodeVerification:{passwordReset.Email}";

                //set Code in Redis for  10mins
                await _redisDatabase.StringSetAsync(verificationKey, code, TimeSpan.FromMinutes(10));

                //increment Rate Limit Counter
                if (!attemptCount.HasValue)
                {
                    await _redisDatabase.StringSetAsync(RateLimitKey, 1, TimeSpan.FromMinutes(10));
                }
                else {
                    await _redisDatabase.StringIncrementAsync(RateLimitKey);
                }

                //send Code Via Email
                await _brevoEmailService.SendAsync(passwordReset.Email, "Password Reset Code", $"Your Password Reset Code is {code}");

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Password Reset Code Sent Successfully"
                };

            }
            catch (Exception ex) {

                _logger.LogError(ex, "Error occurred while sending email verification code to {Email}", passwordReset.Email);

                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage  = "Error Sending Reset Code"
                };

            }
        }

        public async Task<Status> VerifyEmailValidationCode(EmailVerificationCodeDTO verificationCodeDTO)
        {
            try
            {
                //Retrieve Code stored in cache
                var codekey = $"EmailVerification:{verificationCodeDTO.Email}";
                var storedCode = await _redisDatabase.StringGetAsync(codekey);

                //compare stored Code with input Code
                if (storedCode.IsNullOrEmpty || storedCode.ToString().Trim() != verificationCodeDTO.Code.Trim())
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = $"Invalid/Expired Reset Code for {verificationCodeDTO.Email} "//preferably a masked Email
                    };
                }

                //mark as verified and delete Code to prevent Reuse
                var verifiedKey = $"ResetCodeVerified:{verificationCodeDTO.Email}";
                await _redisDatabase.KeyDeleteAsync(codekey);
                //mark key as verified
                await _redisDatabase.StringSetAsync(verifiedKey, "true", TimeSpan.FromMinutes(10));


                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = $" Code Verified,Proceed to Reset your Password"
                };


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying reset code for {Email}", verificationCodeDTO.Email);


                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Internal server error during code verification."

                };

            }
        }

        public async Task<Status> VerifyResetCode(VerifyResetCodeDto verifyResetCode) {
            try
            {
                //Retrieve Code stored in cache
                var codekey = $"ResetCodeVerification:{verifyResetCode.Email}";
                var storedCode = await _redisDatabase.StringGetAsync(codekey);

                //compare stored Code with input Code
                if (storedCode.IsNullOrEmpty || storedCode.ToString().Trim() != verifyResetCode.Code.Trim())
                {
                    return new Status
                    {
                        ResponseCode = 400,
                        ResponseMessage = $"Invalid/Expired Reset Code for {verifyResetCode.Email} "//preferably a masked Email
                    };
                }

                //mark as verified and delete Code to prevent Reuse
                var verifiedKey = $"ResetCodeVerified:{verifyResetCode.Email}";
                await _redisDatabase.KeyDeleteAsync(codekey);
                //mark key as verified
                await _redisDatabase.StringSetAsync(verifiedKey, "true", TimeSpan.FromMinutes(10));


                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = $" Code Verified,Proceed to Reset your Password"
                };
                

            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error verifying reset code for {Email}", verifyResetCode.Email);


                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Internal server error during code verification."

                };

            }
        }

        public async Task<Status> ResetPassword(ResetPasswordDto resetPassword)
        {
            //an array of bytes
            byte[] salt;
            try
            {
                var verifiedKey = $"ResetCodeVerified:{resetPassword.Email}";
                var isVerified = await _redisDatabase.StringGetAsync(verifiedKey);

                if (isVerified.IsNullOrEmpty || isVerified.ToString() != "true") {

                    return new Status
                    {
                        ResponseCode = 403,
                        ResponseMessage = "Unauthorized or expired verification. Please verify the code first."
                    };
                }

                //check if the user Exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == resetPassword.Email);
                if (existingUser == null) {
                    return new Status
                    {
                        ResponseCode = 404,
                        ResponseMessage = "User not found."
                    };
                }

                //update Password
                var newHashedPassword = HashPassword(resetPassword.NewPassword, out salt);
                existingUser.Password = newHashedPassword;
                existingUser.Salt = Convert.ToBase64String(salt);

                _dbContext.Users.Update(existingUser);
                await _dbContext.SaveChangesAsync();

                //remove verification key to invalidate Reset Flow

                await _redisDatabase.KeyDeleteAsync(verifiedKey);

                return new Status
                {
                    ResponseCode = 200,
                    ResponseMessage = "Password reset successfully."
                };
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error resetting password for {Email}", resetPassword.Email);

                return new Status
                {
                    ResponseCode = 500,
                    ResponseMessage = "Internal server error while resetting password."
                };
            }
        }


        public async Task<RegisterResponse> Register(Register register)
        {   //an array of bytes
            byte[] salt;
            try
            {
                //check if user already exists by email
                var existingemail = await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == register.Email);
                if (existingemail != null)
                {

                    return new RegisterResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided Email Already Exists"
                    };
                }
                var existingphone = await _dbContext.Users.FirstOrDefaultAsync(e => e.PhoneNumber == register.PhoneNumber);
                if (existingphone != null)
                {

                    return new RegisterResponse
                    {
                        ResponseCode = 400,
                        ResponseMessage = "User with the Provided phonenumber Already Exists"
                    };

                }

                var hashedPassword = HashPassword(register.password, out salt);

                //create an instamce of userInfo
                var user = new Users
                {

                    UserName = register.UserName,
                    Password = hashedPassword,
                    Email = register.Email,
                    PhoneNumber = register.PhoneNumber,
                    RoleId = "User",
                    IsAdmin = false,
                    CreatedAt = DateTime.UtcNow,
                    IsLoggedIn = false,
                    LastLogin = null,
                    PasswordChangesOn = null,
                    FailedAttempts = 0,
                    Salt = Convert.ToBase64String(salt),
                    isEmailVerified = true,
           


                };


                _dbContext.Users.Add(user);

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
            catch (Exception ex)
            {

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
                    ? await _dbContext.Users.FirstOrDefaultAsync(e => e.Email == userLogin.EmailorPhone)
                    : await _dbContext.Users.FirstOrDefaultAsync(e => e.PhoneNumber == userLogin.EmailorPhone);

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

                _dbContext.Users.Update(user);
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

        public string GenerateRefreshToken()
        {
            using (var rngCryptoServiceProvide = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[32];
                rngCryptoServiceProvide.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public string GenerateJwtToken(Users user)
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

        private bool VerifyPassword(string enteredPassword, string storedHash, byte[] storedSalt)
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

        public string HashPassword(string password, out byte[] salt)
        {

            salt = new byte[128 / 8];

            try
            {
                //Generate a random Number and get bytes
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                    ));

                return hashed;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}
