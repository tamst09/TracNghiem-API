using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FrontEndWebApp.Models;
using FrontEndWebApp.Services;
using System.Security.Claims;

namespace FrontEndWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserClient _userClient;
        public HomeController(IUserClient userClient)
        {
            _userClient = userClient;
        }

        public IActionResult Index()
        {
            ViewData["role"] = "user";
            //if (User.HasClaim(ClaimTypes.Role,"admin"))
            if (User.IsInRole("admin"))
            {
                ViewData["role"] = "admin";
            }
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
