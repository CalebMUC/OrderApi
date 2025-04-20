using Minimart_Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Minimart_Api.Models;
using Minimart_Api.DTOS.Authorization;

namespace Authentication_and_Authorization_Api.Core
{
    public class CoreLibraries
    {

        private readonly IConfiguration _config;


        public CoreLibraries(IConfiguration config)
        {
            _config = config;
        }

        //public static string Encryption(string password) 
        //{

        //}
        //Generates a Token

        //Generate Refresh Token
        public static RefreshTokens GenerateRefreshToken(string userID)
        {

            var refreshToken = new RefreshTokens
            {
                RefreshToken = Guid.NewGuid().ToString(),
                UserName = userID,
                ExpiryDate = DateTime.Now.AddDays(7)
            };




            return refreshToken;

        }

        //public static RefreshTokens GenerateRefreshToken(string userID)
        //{

        //    var refreshToken = new RefreshTokens
        //    {
        //        RefreshToken = Guid.NewGuid().ToString(),
        //        UserName = userID,
        //        ExpiryDate = DateTime.Now.AddDays(7)
        //    };




        //    return refreshToken;

        //}

        public  string GenerateToken(Users user)
        {
            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
            //var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            //var claims = new[]
            //{
            //    new Claim(ClaimTypes.NameIdentifier, userInfo.Name),
            //    new Claim(ClaimTypes.Email, userInfo.Email)
            //    //new Claim(ClaimTypes.Role, userInfo.RoleID)
            //};

            //var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            //    _config["Jwt:Audience"],
            //    claims,
            //    expires: DateTime.Now.AddMinutes(5),
            //    signingCredentials: signingCredentials
            //    );

            //return new JwtSecurityTokenHandler().WriteToken(token);

            //new
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }




        public static CookieOptions SetRefreshToken(RefreshTokens newrefreshToken)
        {

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newrefreshToken.ExpiryDate
            };

            return cookieOptions;
            //Response.Cookies.Append("refereshToken", newrefreshToken.RefreshToken, cookieOptions);

        }

        //public static void SaveRefreshTokens(string RefreshTokenDetails, string myConnectionString)
        //{
        //    using (SqlConnection connection = new SqlConnection(myConnectionString))
        //    {
        //        SqlCommand sqlCommand = new SqlCommand("p_SaveRefreshToken", connection);

        //        sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;

        //        sqlCommand.Parameters.AddWithValue("RefreshTokenDetails", RefreshTokenDetails);


        //        //open connection

        //        connection.Open();

        //        sqlCommand.ExecuteReader();

        //        connection.Close();
        //    }

        //}

        //public static UserInfo GetRefreshToken(string userDetails, string myConnectionString)
        //{

        //    string response = String.Empty;
        //    bool responseCode = false;
        //    string responseMessage = String.Empty;

        //    var usrInfo = new UserInfo();



        //    //try
        //    //{
        //    using (SqlConnection connection = new SqlConnection(myConnectionString))
        //    {
        //        SqlCommand command = new SqlCommand("p_GetRefreshTokens", connection);

        //        command.CommandType = System.Data.CommandType.StoredProcedure;


        //        command.Parameters.AddWithValue("@UserDetails", userDetails);


        //        connection.Open();

        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                responseCode = reader.GetInt32(0) != 0;

        //                responseMessage = reader.GetString(1);

        //                usrInfo = new UserInfo()
        //                {
        //                    Name = reader["UserID"].ToString(),
        //                    Email = reader["PhoneNumber"].ToString(),
        //                    Password = reader["Password"].ToString(),
        //                    RoleID = reader["RoleID"].ToString(),
        //                    status = new ResponseStatus()
        //                    {
        //                        ResponseCode = responseCode,
        //                        ResponseMessage = responseMessage
        //                    }
        //                };




        //            }
        //        }

        //    }











        //    //}
        //    //catch(Exception ex)
        //    //{
        //    //    return ex;
        //    //}

        //    return usrInfo;

        //}

        //public static UserInfo AuthenticateUser(string connectionString, string UserInfoDetails)
        //{
        //    bool ResponseCode = false;
        //    string ResponseMessage = string.Empty;

        //    DataSet dsUsrInfo = new DataSet();

        //    var usrInfo = new UserInfo();

        //    ResponseStatus status = null;

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            SqlCommand command = new SqlCommand("p_AuthenticateUser", connection);

        //            command.CommandType = System.Data.CommandType.StoredProcedure;

        //            command.Parameters.AddWithValue("@UserIDDetails", UserInfoDetails);

        //            connection.Open();

        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    ResponseCode = reader.GetInt32(0) != 0;
        //                    ResponseMessage = reader.GetString(1);

        //                    usrInfo = new UserInfo()
        //                    {
        //                        Name = reader["UserID"].ToString(),
        //                        Email = reader["PhoneNumber"].ToString(),
        //                        Password = reader["Password"].ToString(),
        //                        RoleID = reader["RoleID"].ToString(),
        //                        status = new ResponseStatus()
        //                        {
        //                            ResponseCode = ResponseCode,
        //                            ResponseMessage = ResponseMessage
        //                        }
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    catch (SqlException sqlEx)
        //    {
        //        // Handle SQL exceptions, such as issues with the command or connection
        //        Console.WriteLine($"SQL Error: {sqlEx.Message}");
        //        // You can log the error or rethrow it, depending on your needs
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle any other exceptions
        //        //Console.WriteLine($"An error occurred: {ex.Message}");

        //        status = new ResponseStatus
        //        {
        //            ResponseCode = ResponseCode,
        //            ResponseMessage = ResponseMessage
        //        };

        //        usrInfo.status = status;

        //        return usrInfo;
        //        // You can log the error or rethrow it, depending on your needs
        //    }

        //    return usrInfo;
        //}


        //public static string UserRegistration(string registrationDetails, string myConnectionString)
        //{
        //    string response = String.Empty;
        //    string responseCode = String.Empty;
        //    string responseMessage = String.Empty;

        //    try
        //    {

        //        //log request 
        //        logs.LogRequest(registrationDetails, "requestRegisterLog.txt");
        //        using (SqlConnection connection = new SqlConnection(myConnectionString))
        //        {


        //            SqlCommand command = new SqlCommand("p_AddUser", connection);

        //            command.CommandType = System.Data.CommandType.StoredProcedure;

        //            command.Parameters.AddWithValue("@RegisterDetails", registrationDetails);

        //            connection.Open();



        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    responseCode = reader.GetString(0);
        //                    responseMessage = reader.GetString(1);

        //                }
        //            }

        //            connection.Close();

        //        }

        //        return responseMessage;




        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();

        //    }
        //}
    }
}
