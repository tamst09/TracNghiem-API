using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace TN.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser()
        {
            RefreshTokens = new List<RefreshToken>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public DateTime DoB { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
