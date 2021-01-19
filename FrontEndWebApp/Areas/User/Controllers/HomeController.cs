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
            var allExams = await _examService.GetAllExams(examPagingRequest, token, User.FindFirst("UserID").Value);  // get all exams paged - about 8 exams per page
            //var allCommonExams = // about 8 exams having most attemp
            //var allNewExams = // about 8 exams that time created newest
            if(keyword!=null)
            {
                //allexams = await _categoryService.GetAllExams(Int32.Parse(keyword), token);
            }
            ViewData["allexam"] = allExams.data;
            return View();
        }
    }
}
