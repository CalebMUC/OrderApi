namespace Minimart_Api.TempModels
{
    public class UserRegStatus
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;

        public int? UserID { get; set; }
    }
}
