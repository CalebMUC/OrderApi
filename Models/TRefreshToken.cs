﻿using System;
using System.Collections.Generic;

namespace Minimart_Api.Models
{
    public partial class TRefreshToken
    {
        public string? UserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
