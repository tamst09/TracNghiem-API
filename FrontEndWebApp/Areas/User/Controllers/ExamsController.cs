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
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class ExamsController : Controller
    {
        public IExamService _examService;
        public ICategoryService _categoryService;
        public IQuestionService _questionService;

        public ExamsController(IExamService examService, ICategoryService categoryService, IQuestionService questionService)
        {
            _examService = examService;
            _categoryService = categoryService;
            _questionService = questionService;
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
        [HttpGet("{examID}")]
        public async Task<IActionResult> ManageQuestions(int examID)
        {
            ViewData["msg"] = string.Empty;
            var lstCategory = await _categoryService.GetAll();
            ViewData["lstCategory"] = lstCategory.data;

            var getExamRes = await _examService.GetByID(examID);
            if (getExamRes.success)
            {
                ViewData["examID"] = getExamRes.data.ID;
                ViewData["examName"] = getExamRes.data.ExamName.ToUpper();
                var questions = await _questionService.GetByExamID(examID);
                if(questions.success)
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

        public async Task<IActionResult> AttempQuizOptions([FromQuery]int examID)
        {
            var getExamResponse = await _examService.GetByID(examID);
            if(getExamResponse.success)
            {
                return PartialView(getExamResponse.data);
            }
            return PartialView();
        }

        [HttpPost]
        public IActionResult AttempQuiz([FromQuery]ExamModel model,[FromQuery] bool countdown = false)
        {
            if (model.ID>0)
            {
                //return RedirectToAction("DoingQuiz", new { examID = model.ID });
                return Json(new { examID = model.ID });
            }
            //if (exam != null && exam.data != null)
            //{
            //    return PartialView(exam);
            //}
            return Json(new { });
        }

        public async Task<IActionResult> DoingQuiz([FromQuery] int examID, int pageIndex = 1)
        {
            var pagingRequest = new QuestionPagingRequest()
            {
                ExamID = examID,
                PageIndex = pageIndex,
                PageSize = 5
            };
            var res = await _questionService.GetPagedQuestion(pagingRequest);
            

            if (res != null && res.data != null)
            {
                var examData = res.data;
                if (examData.Items.Count == 0 || examData.Items == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var time = examData.Items.FirstOrDefault().Exam.Time;
                if (time == 0)
                {
                    ViewBag.min = null;
                    ViewBag.sec = null;
                }
                else
                {
                    ViewBag.min = time/60;
                    ViewBag.sec = time%60;
                }
                Global.results = new List<Result>();
                var userID = Int32.Parse(User.FindFirst("UserId").Value);
                foreach (var i in examData.Items)
                {
                    Global.results.Add(new Result() { UserID = userID, QuestionID = i.ID, OptChoose = "N" });
                }
                Global.questions = examData.Items;
                return View(examData);
            }
            return View();
        }

        [HttpPost]
        public void SubmitOne(Result form)
        {
            bool isExist = false;
            var userID = Int32.Parse(User.FindFirst("UserId").Value);
            Result result = new Result()
            {
                UserID = userID,
                QuestionID = form.QuestionID,
                OptChoose = form.OptChoose
            };
            foreach (var i in Global.results)
            {
                if (i.UserID == userID)
                {
                    if (i.QuestionID == result.QuestionID)
                    {
                        i.OptChoose = result.OptChoose;
                        isExist = true;
                    }
                }
            }
            if (!isExist)
            {
                Global.results.Add(result);
                isExist = false;
            }
        }

        public IActionResult SubmitTest()
        {
            ViewData["Score"] = 0;
            int soCauDung = TinhSoCauDung(Int32.Parse(User.FindFirst("UserID").Value));
            if (soCauDung == -1)
                return View();
            else
            {
                ViewData["Score"] = TinhDiem(soCauDung, Global.questions.Count, 10);
            }
            if (Global.results.Count != 0)
                return View(Global.results);
            return View();
        }

        public double TinhDiem(int socaudung, int tongsocau, int thangdiem)
        {
            double k = (double)(socaudung * thangdiem) / tongsocau;
            return Math.Round(k, 2);
        }

        public int TinhSoCauDung(int UID)
        {
            int socauDung = 0;
            List<Result> bailam = new List<Result>();
            foreach (var item in Global.results)    //filter by User ID
            {
                if (item.UserID == UID)
                {
                    bailam.Add(item);
                }
            }
            if (bailam.Count == 0)
            {
                return -1;  // user not found
            }
            else
            {
                foreach (var b in bailam)
                {
                    foreach (var cauhoi in Global.questions)
                    {
                        if (b.QuestionID == cauhoi.ID)
                        {
                            if (b.OptChoose.ToLower() == cauhoi.Answer.ToLower())
                            {
                                socauDung++;
                            }
                        }
                    }
                }
                return socauDung;
            }
        }
    }
}
