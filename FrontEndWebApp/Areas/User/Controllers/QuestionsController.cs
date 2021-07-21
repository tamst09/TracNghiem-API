using ExcelDataReader;
using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IExamService _examService;
        private readonly IHostingEnvironment _appEnvironment;


        public QuestionsController(IQuestionService questionService, IExamService examService, IHostingEnvironment appEnvironment)
        {
            _questionService = questionService;
            _examService = examService;
            _appEnvironment = appEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetByExam([FromQuery] int examID)
        {
            ViewData["msg"] = string.Empty;
            //var lstCategory = await _categoryService.GetAll();
            //ViewData["lstCategory"] = lstCategory.data;

            var getExamRes = await _examService.GetByID(examID);
            if (getExamRes.success)
            {
                ViewData["examID"] = getExamRes.data.ID;
                ViewData["examName"] = getExamRes.data.ExamName.ToUpper();
                var questions = await _questionService.GetByExamID(examID);
                if (questions.success)
                    return View(questions.data);
                else
                {
                    ViewData["msg"] = questions.msg;
                    return View();
                }
            }
            ViewData["msg"] = getExamRes.msg;
            return View();
        }

        public async Task<IActionResult> GetByExamJson([FromQuery] string examId)
        {
            var getExamRes = await _examService.GetByID(int.Parse(examId));
            if (getExamRes.success)
            {
                var questions = await _questionService.GetByExamID(int.Parse(examId));
                if (questions.success)
                {
                    var questionsModel = questions.data.Select(q => new CreateQuestionRequest()
                    {
                        answer = q.Answer,
                        examId = q.ExamID.ToString(),
                        option1 = q.Option1,
                        option2 = q.Option2,
                        option3 = q.Option3,
                        option4 = q.Option4,
                        quesContent = q.QuesContent
                    }).ToList();
                    return Json(questionsModel);
                }
            }
            return Json(new { success = false, msg = "No exam found." });
        }

        public IActionResult Create(int examID)
        {
            ViewData["msg"] = string.Empty;
            return View(new QuestionModel() { ExamID = examID });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
        {
            ViewData["msg"] = string.Empty;

            var createResult = await _questionService.Create(new QuestionModel()
            {
                ExamID = int.Parse(request.examId),
                QuesContent = request.quesContent,
                Option1 = request.option1,
                Option2 = request.option2,
                Option3 = request.option3,
                Option4 = request.option4,
                Answer = request.answer,
                isActive = true
            });

            return Json(createResult);
        }
        public class CreateQuestionRequest
        {
            public string quesContent { get; set; }
            public string option1 { get; set; }
            public string option2 { get; set; }
            public string option3 { get; set; }
            public string option4 { get; set; }
            public string answer { get; set; }
            public string examId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromFile(int examID, IFormCollection collection)
        {
            var files = HttpContext.Request.Form.Files;
            List<AddQuestionRequest> questions = new List<AddQuestionRequest>();
            foreach (var item in files)
            {
                if (item.Length > 0 && item != null)
                {
                    string file_name = Guid.NewGuid().ToString().Replace("-", "") + "_" + item.FileName;
                    string uploads = Path.Combine(_appEnvironment.WebRootPath, "files");
                    string urlPart = uploads + "/" + file_name;
                    string extension = Path.GetExtension(urlPart);
                    if (extension == ".xls" || extension == ".xslx" || extension == ".xlsx")
                    {
                        using (var fileStream = new FileStream(Path.Combine(uploads, file_name), FileMode.Create))
                        {
                            await item.CopyToAsync(fileStream);
                        }
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (var stream = System.IO.File.Open(urlPart, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                do
                                {
                                    while (reader.Read())
                                    {
                                        try
                                        {
                                            questions.Add(new AddQuestionRequest
                                            {
                                                QuesContent = reader.GetValue(0).ToString(),
                                                Option1 = reader.GetValue(1).ToString(),
                                                Option2 = reader.GetValue(2).ToString(),
                                                Option3 = reader.GetValue(3).ToString(),
                                                Option4 = reader.GetValue(4).ToString(),
                                                Answer = reader.GetValue(5).ToString(),
                                                ExamID = examID
                                            });
                                        }
                                        catch (NullReferenceException e)
                                        {
                                            break;
                                        }
                                    }
                                } while (reader.NextResult());
                            }

                        }
                    }
                }
            }
            if (questions.Count > 0)
            {
                AddListQuestionRequest addListQuestionRequest = new AddListQuestionRequest();
                addListQuestionRequest.Questions.AddRange(questions);
                var addResult = await _questionService.AddListQuestions(addListQuestionRequest);
                if (addResult.success)
                {
                    TempData["ImportMsg"] = "Imported!";
                }
                else
                {
                    TempData["ImportMsg"] = "Sorry! Some errors happend!";
                }
            }
            TempData["ImportMsg"] = "This file not supported.";
            return RedirectToAction(nameof(GetByExam), new { examID = examID });
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var delete = await _questionService.Delete(id);
            return Json(delete);
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
        public async Task<IActionResult> Update(QuestionModel model)
        {
            ViewData["msg"] = string.Empty;
            var updateResult = await _questionService.Update(model);
            return RedirectToAction("ManageQuestions", "Exams", new { id = model.ExamID });
        }

        [HttpGet]
        public IActionResult GetListQuestionsViewComp(int examId)
        {
            return ViewComponent("QuestionsByExam", new { examId = examId });
        }
    }
}
