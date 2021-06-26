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
        private readonly IExamManage _examManage;
        private readonly IQuestionManage _quesManage;

        public HomeController(IUserManage userManage, ICategoryManage cateManage, IQuestionManage quesManage, IExamManage examManage)
        {
            _userManage = userManage;
            _cateManage = cateManage;
            _quesManage = quesManage;
            _examManage = examManage;
        }

        public async Task<IActionResult> Index()
        {
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var users = await _userManage.GetNumberOfUsers(access_token);
            var categories = await _cateManage.GetAll();
            var exams = await _examManage.GetAll(access_token);
            var questions = await _quesManage.GetNumberQuestion(access_token);
            ViewBag.TotalExam = exams.data.Count();
            ViewBag.TotalQuestion = questions.data;
            if (users != null && categories !=null)
            {
                if (users.msg!=null)
                {
                    ViewBag.Error = users.msg;
                }
                ViewBag.TotalUser = users.data;
                ViewBag.TotalCategory = categories.data.Count;
            }
            return View();
        }
    }
}
