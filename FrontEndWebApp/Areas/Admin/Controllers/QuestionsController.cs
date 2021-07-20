using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionManage _questionManage;

        public QuestionsController(IQuestionManage questionManage)
        {
            _questionManage = questionManage;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["msg"] = string.Empty;
            var model = new QuestionPagingRequest()
            {
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var getPagedQuestions = await _questionManage.GetAllPaging(model);
            if (!getPagedQuestions.success)
            {
                ViewData["msg"] = getPagedQuestions.msg;
                return View();
            }
            return View(getPagedQuestions.data);
        }

        public IActionResult Create(int examID)
        {
            ViewData["msg"] = string.Empty;
            return PartialView(new QuestionModel() { ExamID = examID });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromQuery] QuestionModel model)
        {
            ViewData["msg"] = "";
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ShowQuestions","Exams", new { id = model.ExamID });
            }

            var createQuestion = await _questionManage.Create(model);
            ViewData["msg"] = createQuestion.msg;

            return RedirectToAction("ShowQuestions", "Exams", new { id = model.ExamID });
        }

        public async Task<IActionResult> Update(int ID)
        {
            ViewData["msg"] = string.Empty;

            var getQuestion = await _questionManage.GetByID(ID);
            if (!getQuestion.success)
            {
                ViewData["msg"] = getQuestion.msg;
                return PartialView(new QuestionModel());
            }
            var model = new QuestionModel()
            {
                ID = getQuestion.data.ID,
                QuesContent = getQuestion.data.QuesContent,
                Option1 = getQuestion.data.Option1,
                Option2 = getQuestion.data.Option2,
                Option3 = getQuestion.data.Option3,
                Option4 = getQuestion.data.Option4,
                Answer = getQuestion.data.Answer,
                ExamID = getQuestion.data.ExamID,
                STT = getQuestion.data.STT
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromQuery]QuestionModel model)
        {
            ViewData["msg"] = string.Empty;

            var updateResult = await _questionManage.Update(model);
            return RedirectToAction("ShowQuestions", "Exams", new { id = model.ExamID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMany([FromBody] int[] s)
        {
            if (s.Length == 0)
            {
                return Json(new { deleteResult = false });
            }
            DeleteManyModel<int> deleteModel = new DeleteManyModel<int>();
            deleteModel.ListItem.AddRange(s);

            var delete = await _questionManage.DeleteMany(deleteModel);
            return Json(new { deleteResult = delete.success });

        }
    }
}
