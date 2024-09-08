using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TCart
    {
        public TCart()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int CartId { get; set; }
        public int? UserId { get; set; }
        public string? CartName { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual TUser? User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
