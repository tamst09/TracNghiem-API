using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TN.WebApplication.Models
{
    public class AppRole : IdentityRole<int>
    {
        public string Description { get; set; }
    }
}
