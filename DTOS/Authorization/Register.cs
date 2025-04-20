namespace Minimart_Api.DTOS.Authorization
{
    public class Register
    {
        public string? UserName { get; set; }
        //can be email or phonenumber
        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? password { get; set; }

        public string? ReEnteredpassword { get; set; }
    }
}
