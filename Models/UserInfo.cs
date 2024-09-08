using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication_and_Authorization_Api.Models
{
    public class UserInfo
    {
        public int UserInfoId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleID { get; set; }
        public int StatusId { get; set; } // Foreign Key
        public virtual ResponseStatus Status { get; set; } // Navigation Property
    }

}
