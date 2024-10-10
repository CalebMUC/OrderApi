using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TSubcategoryid
    {
        public int SubCategoryId { get; set; }
        public string? ProductName { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategory { get; set; }
        public string? ProductType { get; set; }
        public int? CategoryId { get; set; }

        public virtual TCategory? Category { get; set; }
    }
}
