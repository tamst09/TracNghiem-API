﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.User
{
    public class JwtResponse
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public bool isNewLogin { get; set; }
        public string Error { get; set; }
    }
}
