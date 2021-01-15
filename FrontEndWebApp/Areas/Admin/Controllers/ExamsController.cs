using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ExamsController : Controller
    {
        private readonly IExamManage _examService;
        private readonly ICategoryManage _categoryService;
        private readonly IUserManage _userManage;

        public ExamsController(IExamManage examService, ICategoryManage categoryService, IUserManage userManage)
        {
            _examService = examService;
            _categoryService = categoryService;
            _userManage = userManage;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var allExam = await  _examService.GetAll(accessToken);
            if (allExam.msg != null)
            {
                ViewData["msg"] = allExam.msg;
                return View();
            }
            return View(allExam.data);   
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamModel model)
        {
            if (string.IsNullOrEmpty(model.ExamName))
            {
                return View(model);
            }
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var allExam = await _examService.Create(
                model,
                Int32.Parse(User.FindFirst("UserID").Value),
                accessToken);
            if (allExam!=null && allExam.msg != null)
            {
                ViewData["msg"] = allExam.msg;
                return View();
            }
            else if(allExam==null)
            {
                ViewData["msg"] = "Error";
                return View();
            }
            else
            {
                ViewData["msg"] = "Thêm mới thành công";
                return View();
            }
        }
    }
}
