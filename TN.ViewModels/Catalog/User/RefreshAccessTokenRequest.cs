using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.User
{
    public class RefreshAccessTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
