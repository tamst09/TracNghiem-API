using System;
using System.Collections.Generic;
using System.Text;
using TN.Data.Entities;

namespace TN.ViewModels.FacebookAuth
{
    public class CreateFacebookUserResult
    {
        public AppUser User { get; set; }
        public bool isNewUser { get; set; }
    }
}
