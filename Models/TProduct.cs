using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal Price { get; set; }
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
    }
}
