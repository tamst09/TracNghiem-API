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
            ViewData["msg"] = string.Empty;
            return PartialView(new QuestionModel() { ExamID = examID });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] QuestionModel model)
        {
            ViewData["msg"] = string.Empty;

            var createResponse = await _questionService.Create(model);
            ViewData["msg"] = createResponse.msg;

            return RedirectToAction("ManageQuestions", "Exams", new { examID = model.ExamID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] int[] s)
        {
            try
            {
                if (s.Length == 0)
                {
                    return Json(new { deleteResult = false });
                }
                DeleteManyModel<int> questionIDs = new DeleteManyModel<int>();
                questionIDs.ListItem.AddRange(s);
                var result = await _questionService.DeleteMany(questionIDs);
                return Json(new { deleteResult = result.success });
            }
            catch
            {
                return Json(new { deleteResult = false });
            }
        }

        public async Task<IActionResult> Update(int ID)
        {
            ViewData["msg"] = string.Empty;

            var res = await _questionService.GetByID(ID);
            if (!res.success)
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

        [HttpPost]
        public async Task<IActionResult> Update([FromQuery] QuestionModel model)
        {
            ViewData["msg"] = string.Empty;
            var updateResult = await _questionService.Update(model);
            return RedirectToAction("ManageQuestions", "Exams", new { id = model.ExamID });
        }
    }
}
