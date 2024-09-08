using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication_and_Authorization_Api.Models
{
    public class Register
    {
        public string? UserName { get; set; }
       //can be email or phonenumber
        public string? Email{ get; set; }

        public string? PhoneNumber { get; set; }

        public string? password { get; set; }

        public string? ReEnteredpassword { get; set; }


    }
}
