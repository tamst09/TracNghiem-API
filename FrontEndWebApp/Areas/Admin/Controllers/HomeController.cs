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
            var countUserResponse = await _userManage.CountUser();
            var countCategoryResponse = await _cateManage.Count();
            var countExamResponse = await _examManage.CountExam();
            var countQuestionResponse = await _quesManage.CountQuestion();
            if (!countUserResponse.success)
            {
                ViewBag.Error = "Load user: " + countUserResponse.msg;
            }
            if (!countCategoryResponse.success)
            {
                ViewBag.Error = "Load category: " + countCategoryResponse.msg;
            }
            if (!countExamResponse.success)
            {
                ViewBag.Error = "Load exam: " + countExamResponse.msg;
            }
            if (!countQuestionResponse.success)
            {
                ViewBag.Error = "Load question: " + countQuestionResponse.msg;
            }
            ViewBag.TotalUser = countUserResponse.data;
            ViewBag.TotalCategory = countCategoryResponse.data;
            ViewBag.TotalExam = countExamResponse.data;
            ViewBag.TotalQuestion = countQuestionResponse.data;
            return View();
        }
    }
}
