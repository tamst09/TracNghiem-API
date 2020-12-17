using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Common
{
    public class RefreshAccessTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
