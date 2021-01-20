using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize(Roles = "admin,user")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(int examID)
        {
            ViewData["msg"] = "";
            return PartialView(new QuestionModel() { ExamID = examID });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] QuestionModel model)
        {
            ViewData["msg"] = "";
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageQuestions", "Exams", new { id = model.ExamID });
            }
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var newQuestion = await _questionService.Create(model, accessToken);
            if (newQuestion != null && newQuestion.msg != null)
            {
                ViewData["msg"] = newQuestion.msg;
            }
            else if (newQuestion == null)
            {
                ViewData["msg"] = "Created unsuccessfully";
            }
            else
            {
                ViewData["msg"] = "Created successfully";
            }
            //return Json(new { msg = "OK" });
            return RedirectToAction("ManageQuestions", "Exams", new { id = model.ExamID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] int[] s)
        {
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                if (s.Length == 0)
                {
                    return Json(new { deleteResult = false });
                }
                DeleteRangeModel<int> temp = new DeleteRangeModel<int>();
                temp.ListItem = new List<int>();
                temp.ListItem.AddRange(s);
                var result = await _questionService.DeleteMany(temp, token);
                if (result.msg != null)
                {
                    return Json(new { deleteResult = false });
                }
                return Json(new { deleteResult = true });
            }
            catch
            {
                //return RedirectToAction("Index");
                return Json(new { deleteResult = false });
            }
        }

        public async Task<IActionResult> Update(int ID)
        {
            ViewData["msg"] = "";
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var res = await _questionService.GetByID(ID, accessToken);
            if (res == null)
            {
                ViewData["msg"] = "Lỗi không thể tìm thấy";
                return PartialView(new QuestionModel());
            }
            else
            {
                if (res.msg != null)
                {
                    ViewData["msg"] = res.msg;
                    return PartialView(new QuestionModel());
                }
                else
                {
                    var model = new QuestionModel()
                    {
                        ID = res.data.ID,
                        QuesContent = res.data.QuesContent,
                        Option1 = res.data.Option1,
                        Option2 = res.data.Option2,
                        Option3 = res.data.Option3,
                        Option4 = res.data.Option4,
                        Answer = res.data.Answer,
                        ExamID = res.data.ExamID,
                        STT = res.data.STT
                    };
                    return PartialView(model);
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update([FromQuery] QuestionModel model)
        {
            ViewData["msg"] = "";
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var updateResult = await _questionService.Update(model, accessToken);
            return RedirectToAction("ManageQuestions", "Exams", new { id = model.ExamID });
        }
    }
}
