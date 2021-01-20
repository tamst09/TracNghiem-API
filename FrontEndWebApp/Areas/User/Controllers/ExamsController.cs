using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Exams;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "admin,user")]
    public class ExamsController : Controller
    {
        public IExamService _examService;
        public ICategoryService _categoryService;

        public ExamsController(IExamService examService, ICategoryService categoryService)
        {
            _examService = examService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            
            var examResponse = await _examService.GetOwnedExams(token, User.FindFirst("UserID").Value);
            if(examResponse!=null && examResponse.data != null)
            {
                return View(examResponse.data);
            }
            ViewData["msg"] = "Oh sorry!! Something went wrong";
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var lstCategory = await _categoryService.GetAll();
            ViewData["lstCategory"] = lstCategory.data;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ExamModel model)
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var lstCategory = await _categoryService.GetAll();
            ViewData["lstCategory"] = lstCategory.data;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var newExam = await _examService.Create(model, Int32.Parse(User.FindFirst("UserID").Value), token);
            if(newExam!=null && newExam.data != null)
            {
                ViewData["msg"] = "Successfully";
                return View();
            }
            ViewData["msg"] = "Create failed";
            return View();
        }

        public async Task<IActionResult> ManageQuestions(int examID)
        {
            var lstCategory = await _categoryService.GetAll();
            ViewData["lstCategory"] = lstCategory.data;
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var exam = await _examService.GetByID(examID, token, User.FindFirst("UserID").Value);
            if (exam != null && exam.msg == null && exam.data != null)
            {
                ViewData["examID"] = exam.data.ID;
                ViewData["examName"] = exam.data.ExamName.ToUpper();
                return View(exam.data.Questions.Where(q => q.isActive).ToList());
            }
            ViewData["msg"] = "Sorry!! Something went wrong. Please try again.";
            return View();
        }
    }
}
