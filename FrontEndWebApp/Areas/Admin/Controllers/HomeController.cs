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
        private readonly IQuestionManage _quesManage;

        public HomeController(IUserManage userManage, ICategoryManage cateManage, IQuestionManage quesManage)
        {
            _userManage = userManage;
            _cateManage = cateManage;
            _quesManage = quesManage;
        }

        public async Task<IActionResult> Index()
        {
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var totalUser = await _userManage.GetNumberOfUsers(access_token);
            var totalCategory = await _cateManage.GetAll();
            var totalExam = 0;
            foreach(var category in totalCategory.data)
            {
                totalExam += category.Exams.Where(e => e.isActive == true).ToList().Count;
            }
            var totalQuestion = await _quesManage.GetNumberQuestion(access_token);
            ViewData["Error"] = "";
            ViewData["TotalUser"] = null;
            ViewData["TotalCategory"] = null;
            ViewData["TotalExam"] = totalExam;
            ViewData["TotalQuestion"] = totalQuestion.data;
            if (totalUser != null && totalCategory !=null)
            {
                if (totalUser.msg!=null)
                {
                    ViewData["Error"] = totalUser.msg;
                }
                ViewData["TotalUser"] = totalUser.data;
                ViewData["TotalCategory"] = totalCategory.data.Count;
            }
            return View();
        }
    }
}
