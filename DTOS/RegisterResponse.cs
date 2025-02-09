namespace Minimart_Api.DTOS
{
    public class RegisterResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int? UserID { get; set; }
        public string Username { get; set; }
    }
}
