using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TImage
    {
        public int ImageId { get; set; }
        public string ImageType { get; set; } = null!;
        public byte[] ImageUrl { get; set; } = null!;
    }
}
