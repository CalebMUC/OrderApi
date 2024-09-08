using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication_and_Authorization_Api.Models
{
    public class ResponseStatus
    {
        public int ResponseStatusId { get; set; } // Primary Key
        public bool ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public virtual ICollection<UserInfo> Users { get; set; } // Navigation Property
    }

}
