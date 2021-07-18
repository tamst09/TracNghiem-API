using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FrontEndWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    if (User.IsInRole("admin"))
            //    {
            //        return RedirectToAction("Index", "Home", new { Area = "Admin" });
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index", "Home", new { Area = "User" });
            //    }
            //}
            return View();
        }
        public IActionResult Error()
        {

            ViewData["statusCode"] = HttpContext.Response.StatusCode;
            switch (HttpContext.Response.StatusCode)
            {
                case 500:
                    ViewData["message"] = "Server not avaible";
                    break;
                case 401:
                    ViewData["message"] = "Unauthorized";
                    break;
                case 403:
                    ViewData["message"] = "Forbiden";
                    break;
                case 404:
                    ViewData["message"] = "Page not found";
                    break;
                default:
                    ViewData["message"] = "Something went wrong";
                    break;
            }

            return View();
        }
    }
}
