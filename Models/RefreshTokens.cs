using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication_and_Authorization_Api.Models
{
    public class RefreshTokens
    {
        public string RefreshToken { get; set; }

        public string UserName { get; set; }

        public DateTime  ExpiryDate { get; set; }
    }
}
