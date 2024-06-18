using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TUser
    {
        public string? Name { get; set; }
        public string? EmailAddress { get; set; }
        public string? Role { get; set; }
        public string? Password { get; set; }
        public bool? IsloggedIn { get; set; }
    }
}
