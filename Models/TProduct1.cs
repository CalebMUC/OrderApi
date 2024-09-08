using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TProduct1
    {
        public TProduct1()
        {
            CartItems = new HashSet<CartItem>();
            TOrderItems = new HashSet<TOrderItem>();
            TReviews = new HashSet<TReview>();
        }

        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int StockQuantity { get; set; }
        public int? CategoryId { get; set; }
        public int RowId { get; set; }
        public string ProductId { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string ImageType { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public int InStock { get; set; }
        public string KeyFeatures { get; set; } = null!;
        public string Specification { get; set; } = null!;
        public string Box { get; set; } = null!;
        public string? SubCategoryId { get; set; }
        public string? ProductType { get; set; }

        public virtual TCategory? CategoryNavigation { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<TOrderItem> TOrderItems { get; set; }
        public virtual ICollection<TReview> TReviews { get; set; }
    }
}
