using System;
using System.Collections.Generic;
using System.Text;

namespace TN.Data.Model
{
    public class JwtResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string AccessToken { get; set; }
        public DateTime DoB { get; set; }
    }
}
