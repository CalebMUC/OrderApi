using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TOrderItem
    {
        public int OrderItemId { get; set; }
        public int? OrderId { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual TOrder? Order { get; set; }
        public virtual TProduct1? Product { get; set; }
    }
}
