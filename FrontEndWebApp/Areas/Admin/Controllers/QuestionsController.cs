using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Question;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionManage _questionManage;

        public QuestionsController(IQuestionManage questionManage)
        {
            _questionManage = questionManage;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["msg"] = "";
            try
            {
                var model = new QuestionPagingRequest()
                {
                    keyword = keyword,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var allQuestions = await _questionManage.GetAllPaging(model, token);
                if(allQuestions.msg != null)
                {
                    ViewData["msg"] = allQuestions.msg;
                    return View();
                }
                return View(allQuestions.data);
            }
            catch (Exception)
            {
                ViewData["msg"] = "Lỗi hệ thống. Vui lòng đăng nhập lại.";
                return View();
            }
        }

        public IActionResult Create(int examID)
        {
            ViewData["msg"] = "";
            return View(new QuestionModel() { ExamID = examID });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            ViewData["msg"] = "";
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var newQuestion = await _questionManage.Create(model, accessToken);
            if (newQuestion != null && newQuestion.msg != null)
            {
                ViewData["msg"] = newQuestion.msg;
                return View();
            }
            else if (newQuestion == null)
            {
                ViewData["msg"] = "Thêm mới thất bại";
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
