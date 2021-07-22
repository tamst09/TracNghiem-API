using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Catalog.Result;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class ExamsController : Controller
    {
        private readonly IExamService _examService;
        private readonly ICategoryService _categoryService;
        private readonly IQuestionService _questionService;
        private readonly IResultService _resultService;

        public ExamsController(IExamService examService, ICategoryService categoryService, IQuestionService questionService, IResultService resultService)
        {
            _examService = examService;
            _categoryService = categoryService;
            _questionService = questionService;
            _resultService = resultService;
        }

        public async Task<IActionResult> Index()
        {
            var examResponse = await _examService.GetOwned();
            if (examResponse.success)
            {
                return View(examResponse.data);
            }
            ViewData["msg"] = examResponse.msg;
            return View();
        }

        public async Task<IActionResult> Create()
        {
            var getCategoriesRes = await _categoryService.GetAll();
            ViewData["lstCategory"] = getCategoriesRes.data;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ExamModel model)
        {
            var getCategoriesRes = await _categoryService.GetAll();
            ViewData["lstCategory"] = getCategoriesRes.data;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var createResponse = await _examService.Create(model);
            ViewData["msg"] = createResponse.msg;
            return View();
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
                var result = await _examService.DeleteMany(questionIDs);
                return Json(new { deleteResult = result.success });
            }
            catch
            {
                return Json(new { deleteResult = false });
            }
        }

        public async Task<IActionResult> AttempQuizOptions([FromQuery]int examID)
        {
            var getExamResponse = await _examService.GetByID(examID);
            if(getExamResponse.success)
            {
                var option = new ExamOptions()
                {
                    CountQuestion = getExamResponse.data.Questions.Count,
                    Minute = getExamResponse.data.Time / 60,
                    Second = getExamResponse.data.Time % 60,
                    NoCountDown = getExamResponse.data.Time == 0 ? true : false,
                    ExamId = getExamResponse.data.ID
                };
                return PartialView(option);
            }
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> OnPostQuizOptions(ExamOptions options)
        {
            return RedirectToAction(nameof(AttempQuiz), options);
        }

        [HttpGet]
        public async Task<IActionResult> AttempQuiz(ExamOptions options, int pageIndex=1)
        {
            ViewBag.ExamId = options.ExamId;
            var pagingRequest = new QuestionPagingRequest()
            {
                ExamID = options.ExamId,
                PageIndex = pageIndex,
                PageSize = 5
            };
            var res = await _questionService.GetPagedQuestion(pagingRequest);
            if (res.success)
            {
                var questionPaged = res.data;
                if (questionPaged.Items.Count == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (options.NoCountDown)
                {
                    ViewBag.min = 0;
                    ViewBag.sec = 0;
                }
                else
                {
                    ViewBag.min = options.Minute;
                    ViewBag.sec = options.Second;
                }
                var temp = new List<AddResultRequest>();
                return View(questionPaged);
            }
            return View();
        }

        public async Task<IActionResult> ListQuestionPartial(int examId, int pageIndex = 1)
        {
            var pagingRequest = new QuestionPagingRequest()
            {
                ExamID = examId,
                PageIndex = pageIndex,
                PageSize = 5
            };
            var res = await _questionService.GetPagedQuestion(pagingRequest);
            if (res.success)
            {
                return PartialView("_ListDoingQuestions", res.data);
            }
            return PartialView("_ListDoingQuestions");
        }

        //public async Task<IActionResult> DoingQuiz([FromQuery] int examID, int pageIndex = 1)
        //{
        //    var pagingRequest = new QuestionPagingRequest()
        //    {
        //        ExamID = examID,
        //        PageIndex = pageIndex,
        //        PageSize = 5
        //    };
        //    var res = await _questionService.GetPagedQuestion(pagingRequest);


        //    if (res != null && res.data != null)
        //    {
        //        var examData = res.data;
        //        if (examData.Items.Count == 0 || examData.Items == null)
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }
        //        var time = examData.Items.FirstOrDefault().Exam.Time;
        //        if (time == 0)
        //        {
        //            ViewBag.min = null;
        //            ViewBag.sec = null;
        //        }
        //        else
        //        {
        //            ViewBag.min = time/60;
        //            ViewBag.sec = time%60;
        //        }
        //        return View(examData);
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public void SubmitOne(Result form)
        //{
        //    bool isExist = false;
        //    var userID = Int32.Parse(User.FindFirst("UserId").Value);
        //    Result result = new Result()
        //    {
        //        UserID = userID,
        //        QuestionID = form.QuestionID,
        //        OptChoose = form.OptChoose
        //    };
        //    foreach (var i in Global.results)
        //    {
        //        if (i.UserID == userID)
        //        {
        //            if (i.QuestionID == result.QuestionID)
        //            {
        //                i.OptChoose = result.OptChoose;
        //                isExist = true;
        //            }
        //        }
        //    }
        //    if (!isExist)
        //    {
        //        Global.results.Add(result);
        //        isExist = false;
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamModel submitExamModel)
        {
            if (submitExamModel == null)
            {
                return Json("Null.");
            }
            var addResultRequests = submitExamModel.Results.Select(t => new AddResultRequest()
            {
                Choice = t.choice.ToUpper(),
                QuestionId = int.Parse(t.questionId)
            }).ToList();

            AddListResultRequest request = new AddListResultRequest();
            request.ResultRequests = addResultRequests;
            var submit = await _resultService.AddListResult(request);
            if (submit.success)
            {
                // Get Score
            }
            
            return Json("OK");
        }

        public class SubmitExamModel
        {
            public List<ResultRequest> Results { get; set; }
            public SubmitExamModel()
            {
                Results = new List<ResultRequest>();
            }
        }
        public class ResultRequest
        {
            public string questionId { get; set; }
            public string choice { get; set; }
        }

        //public double TinhDiem(int socaudung, int tongsocau, int thangdiem)
        //{
        //    double k = (double)(socaudung * thangdiem) / tongsocau;
        //    return Math.Round(k, 2);
        //}

        //public int TinhSoCauDung(int UID)
        //{
        //    int socauDung = 0;
        //    List<Result> bailam = new List<Result>();
        //    foreach (var item in Global.results)    //filter by User ID
        //    {
        //        if (item.UserID == UID)
        //        {
        //            bailam.Add(item);
        //        }
        //    }
        //    if (bailam.Count == 0)
        //    {
        //        return -1;  // user not found
        //    }
        //    else
        //    {
        //        foreach (var b in bailam)
        //        {
        //            foreach (var cauhoi in Global.questions)
        //            {
        //                if (b.QuestionID == cauhoi.ID)
        //                {
        //                    if (b.OptChoose.ToLower() == cauhoi.Answer.ToLower())
        //                    {
        //                        socauDung++;
        //                    }
        //                }
        //            }
        //        }
        //        return socauDung;
        //    }
        //}
    }
}
