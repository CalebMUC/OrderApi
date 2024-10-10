using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TOrder
    {
        public TOrder()
        {
            TOrderItems = new HashSet<TOrderItem>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string? Status { get; set; }
        public decimal? TotalAmount { get; set; }

        public virtual TUser? User { get; set; }
        public virtual ICollection<TOrderItem> TOrderItems { get; set; }
    }
}
