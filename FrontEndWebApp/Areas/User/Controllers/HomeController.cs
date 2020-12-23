using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "admin,user")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "User dashboard";
            return View();
        }
    }
}
