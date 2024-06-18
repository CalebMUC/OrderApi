using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TStockDetail
    {
        public string? Symbol { get; set; }
        public string? Name { get; set; }
        public string? Exchange { get; set; }
        public string? AssetType { get; set; }
        public string? Ipodate { get; set; }
    }
}
