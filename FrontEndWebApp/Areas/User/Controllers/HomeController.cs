using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
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
            var allPagedExams = await _examService.GetAllPaging(examPagingRequest, token, User.FindFirst("UserID").Value);
            // get all exams paged - about 8 exams per page
            var allExams = await _examService.GetAll(token, User.FindFirst("UserID").Value);
            if(allExams == null)
            {
                allExams = new TN.ViewModels.Common.ResponseBase<List<TN.Data.Entities.Exam>>()
                {
                    data = new List<TN.Data.Entities.Exam>(),
                    msg = "",
                    success = true
                };
            }
            var commonExams = allExams.data.OrderByDescending(e => e.NumOfAttemps).Take(8).ToList();
            var newestExams = allExams.data.OrderByDescending(e => e.TimeCreated).Take(8).ToList();
            // about 8 exams having most attemp
            //var allNewExams = // about 8 exams that time created newest
            ViewData["Title"] = "HOME";
            ViewBag.CommonExams = commonExams;
            ViewBag.NewestExams = newestExams;
            ViewBag.AllExams = new PagedResult<TN.Data.Entities.Exam>()
            {
                Items = allExams.data,
                PageIndex = 1,
                PageSize = 10,
                TotalPages = allExams.data.Count()/10,
                TotalRecords = allExams.data.Count()
            };
            ViewBag.Categories = allcategory;
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
