using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("admin")]
    public class HomeController : Controller
    {
        private readonly IUserManage _userManage;
        private readonly ICategoryManage _cateManage;

        public HomeController(IUserManage userManage, ICategoryManage cateManage)
        {
            _userManage = userManage;
            _cateManage = cateManage;
        }

        public async Task<IActionResult> Index()
        {
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var numberUser = await _userManage.GetNumberOfUsers(access_token);
            var numberCategory = await _cateManage.GetAll();
            ViewData["Error"] = "";
            ViewData["TotalUser"] = null;
            ViewData["TotalCategory"] = null ;
            if (numberUser != null && numberCategory !=null)
            {
                if (numberUser.msg!=null)
                {
                    ViewData["Error"] = numberUser.msg;
                }
                ViewData["TotalUser"] = numberUser.data;
                ViewData["TotalCategory"] = numberCategory.data.Count;
            }
            return View();
        }
    }
}
