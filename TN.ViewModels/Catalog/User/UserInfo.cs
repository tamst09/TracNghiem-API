using System;
using System.Collections.Generic;
using System.Text;

namespace TN.ViewModels.Catalog.User
{
    public class UserInfo
    {
        public UserInfo()
        {
            Roles = new List<string>();
        }

        public int UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public List<string> Roles { get; set; }
    }
}
