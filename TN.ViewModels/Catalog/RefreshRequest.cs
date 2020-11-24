using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog
{
    public class RefreshRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
