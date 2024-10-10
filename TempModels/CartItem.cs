using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class CartItem
    {
        public int CartItemId { get; set; }
        public int? CartId { get; set; }
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public virtual TCart? Cart { get; set; }
        public virtual TProduct? Product { get; set; }
    }
}
