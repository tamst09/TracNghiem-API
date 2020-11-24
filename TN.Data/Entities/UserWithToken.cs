using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Entities
{
    public class UserWithToken : AppUser
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public UserWithToken(AppUser user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.DoB = user.DoB;
            this.PhoneNumber = user.PhoneNumber;
            this.UserName = user.UserName;
        }
    }
}
