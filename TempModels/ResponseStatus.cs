using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimart_Api.TempModels
{
    public class ResponseStatus
    {
        public int ResponseStatusId { get; set; }
        public bool ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public virtual ICollection<UserInfo> Users { get; set; }
    }

}
