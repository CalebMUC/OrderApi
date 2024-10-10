using System;
using System.Collections.Generic;

namespace Minimart_Api.TempModels
{
    public partial class TImage
    {
        public int ImageId { get; set; }
        public string ImageType { get; set; } = null!;
        public byte[] ImageUrl { get; set; } = null!;
    }
}
