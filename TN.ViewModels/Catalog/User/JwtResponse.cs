using System;
using System.Collections.Generic;
using System.Text;
using TN.ViewModels.Common;

namespace TN.ViewModels.Catalog.User
{
    public class JwtResponse
    {
        public string Access_Token { get; set; }
        public string Refresh_Token { get; set; }
        public bool isNewLogin { get; set; }
    }
}
