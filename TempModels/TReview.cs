using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TReview
    {
        public int ReviewId { get; set; }
        public string? ProductId { get; set; }
        public int? UserId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public byte[] ReviewDate { get; set; } = null!;

        public virtual TProduct? Product { get; set; }
        public virtual TUser? User { get; set; }
    }
}
