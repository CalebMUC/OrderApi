﻿namespace Minimart_Api.DTOS.Authorization
{
    public class UserRegStatus
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; } = string.Empty;

        public int? UserID { get; set; }
    }
}
