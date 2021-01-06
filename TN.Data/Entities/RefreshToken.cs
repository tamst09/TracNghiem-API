﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
        public AppUser User { get; set; }
    }
}
