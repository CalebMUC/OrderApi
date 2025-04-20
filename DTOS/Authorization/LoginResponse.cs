
namespace Minimart_Api.DTOS.Authorization
{
    public class LoginResponse
    {
        public int? UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserRole { get; set; }

        public string Token { get; set; }

        public string Refreshtoken { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        //public static LoginResponseStatus status { get; set; }
    }
}
