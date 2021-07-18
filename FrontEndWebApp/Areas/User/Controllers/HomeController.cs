using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        public ICategoryService _categoryService;
        public IExamService _examService;
        public IQuestionService _questionService;

        public HomeController(ICategoryService categoryService, IExamService examService, IQuestionService questionService)
        {
            _categoryService = categoryService;
            _examService = examService;
            _questionService = questionService;
        }

        public async Task<IActionResult> Index(string keyword, string categoryId, int pageIndex = 1, int pageSize = 20)
        {
            var getAllCategoriesResponse = await _categoryService.GetAll();  // to show categories
            
            var examPagingRequest = new ExamPagingRequest()
            {
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            if(!string.IsNullOrWhiteSpace(categoryId))
            {
                examPagingRequest.CategoryID = int.Parse(categoryId);
            }
            var getAllExamsPagedResponse = await _examService.GetAllPaging(examPagingRequest);

            var getAllExamsResponse = await _examService.GetAll();

            ViewBag.CommonExams = new List<Exam>();
            ViewBag.NewestExams = new List<Exam>();
            ViewBag.AllExams = new PagedResult<Exam>();
            ViewBag.Categories = new List<Category>();

            if (getAllExamsResponse.success)
            {
                var commonExams = getAllExamsResponse.data.OrderByDescending(e => e.NumOfAttemps).Take(8).ToList();
                var newestExams = getAllExamsResponse.data.OrderByDescending(e => e.TimeCreated).Take(8).ToList();
                ViewBag.CommonExams = commonExams;
                ViewBag.NewestExams = newestExams;
                
            }
            if (getAllExamsPagedResponse.success)
            {
                ViewBag.AllExams = getAllExamsPagedResponse.data;
            }
            if (getAllCategoriesResponse.success)
            {
                ViewBag.Categories = getAllCategoriesResponse.data;
            }

            ViewData["Title"] = "HOME";
            return View();
        }

        [HttpGet("PreviewExam")]
        public async Task<IActionResult> PreviewExam(int examID)
        {
            var exam = await _examService.GetByID(examID);
            if (exam.success)
            {
                ViewData["examName"] = exam.data.ExamName;
                var getQuestionsRes = await _questionService.GetByExamID(examID);
                if(getQuestionsRes.success)
                    return View(getQuestionsRes.data);
            }
            return View();
        }
    }
}
