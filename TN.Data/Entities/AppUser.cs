using Microsoft.AspNetCore.Identity;
using System;

namespace TN.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime DoB { get; set; }
        public bool isActive { get; set; }
        public string RefreshTokenValue { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
