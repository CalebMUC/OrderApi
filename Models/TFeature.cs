using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TFeature
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? MainCategory { get; set; }
        public string Resolution { get; set; } = null!;
        public string ScreenSize { get; set; } = null!;
        public int? RefreshRate { get; set; }
        public string? ResponseTime { get; set; }
        public string? AdaptiveSync { get; set; }
        public string? ConnectiveTechnology { get; set; }
        public string? SpecialFeatures { get; set; }
        public string? ScreenSurface { get; set; }
        public string? MountingType { get; set; }
        public string? ImageBrightness { get; set; }
        public string? ItemWeight { get; set; }
        public string? DisplayResolution { get; set; }
        public string? Color { get; set; }
        public string? WarrantyType { get; set; }
        public string? DisplayType { get; set; }
        public string? Brand { get; set; }
    }
}
