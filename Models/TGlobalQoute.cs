using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TGlobalQoute
    {
        public string? Symbol { get; set; }
        public string? Opening { get; set; }
        public string? High { get; set; }
        public string? Low { get; set; }
        public string? Price { get; set; }
        public string? Volume { get; set; }
        public string? TradingDay { get; set; }
        public string? PreviousClose { get; set; }
        public string? Change { get; set; }
        public string? ChangePercent { get; set; }
    }
}
