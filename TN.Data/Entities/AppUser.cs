using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;

namespace TN.Data.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public DateTime DoB { get; set; }
        public bool isActive { get; set; }
        public string RefreshTokenValue { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}
