using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ActionResult> Index()
        {
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            //var user = await _userManage.GetOneUser(access_token, Int32.Parse(User.FindFirst("UserID").Value));
            var numberUser = await _userManage.GetNumberOfUsers(access_token);
            var numberCategory = await _cateManage.GetAll();
            //Global.Avatar_Url = user.Avatar;
            if (numberUser == null)
            {
                numberUser = new TN.ViewModels.Common.NumberUserInfo();
                ViewData["msg"] = "Error";
            }
            ViewData["TotalUser"] = numberUser;
            ViewData["TotalCategory"] = numberCategory.Count;
            return View();
        }
    }
}
