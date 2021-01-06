using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { Area = "User" });
                }
            }
            return View();
        }
    }
}
