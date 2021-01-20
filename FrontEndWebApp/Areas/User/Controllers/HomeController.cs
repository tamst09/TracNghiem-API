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
    public class HomeController : Controller
    {
        public ICategoryService _categoryService;
        public IExamService _examService;

        public HomeController(ICategoryService categoryService, IExamService examService)
        {
            _categoryService = categoryService;
            _examService = examService;
        }

        public async Task<IActionResult> Index(string keyword, string categoryId, int pageIndex = 1, int pageSize = 20)
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var allcategory = await _categoryService.GetAll();  // to show categories
            var examPagingRequest = new ExamPagingRequest()
            {
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            if(categoryId != null)
            {
                examPagingRequest.CategoryID = Int32.Parse(categoryId);
            }
            var allExams = await _examService.GetAllExams(examPagingRequest, token, User.FindFirst("UserID").Value);
            // get all exams paged - about 8 exams per page
            var allExamsNotPaged = await _examService.GetAll(token, User.FindFirst("UserID").Value);
            var commonExams = allExamsNotPaged.data.OrderByDescending(e => e.NumOfAttemps).Take(8).ToList();
            var newestExams = allExamsNotPaged.data.OrderByDescending(e => e.TimeCreated).Take(8).ToList();
            // about 8 exams having most attemp
            //var allNewExams = // about 8 exams that time created newest
            ViewData["commonExams"] = commonExams;
            ViewData["newestExams"] = newestExams;
            ViewData["allExams"] = allExams.data;
            ViewData["Title"] = "HOME";
            return View();
        }
        [HttpGet("PreviewExam")]
        public async Task<IActionResult> PreviewExam(string examID)
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var exam = await _examService.GetByID(Int32.Parse(examID), token, User.FindFirst("UserID").Value);
            if (exam != null && exam.msg == null && exam.data != null)
            {
                ViewData["examName"] = exam.data.ExamName;
                return View(exam.data.Questions.Where(q=>q.isActive).ToList());
            }
            return View();
        }
    }
}
