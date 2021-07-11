using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ExamsController : Controller
    {
        private readonly IExamManage _examService;
        private readonly ICategoryManage _categoryService;
        private readonly IUserManage _userManage;
        private readonly IQuestionManage _questionManage;

        public ExamsController(IExamManage examService, ICategoryManage categoryService, IUserManage userManage, IQuestionManage questionManage)
        {
            _examService = examService;
            _categoryService = categoryService;
            _userManage = userManage;
            _questionManage = questionManage;
        }

        public async Task<IActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            try
            {
                var model = new ExamPagingRequest()
                {
                    keyword = keyword,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var allExam = await _examService.GetAllPaging(model, accessToken);
                if (allExam.msg != null)
                {
                    ViewData["msg"] = allExam.msg;
                    return View();
                }
                allExam.data.Items = allExam.data.Items.OrderByDescending(e => e.TimeCreated).ToList();
                return View(allExam.data);
            }
            catch (Exception)
            {
                return View();
            }   
        }

        public IActionResult Create()
        {
            ViewData["msg"] = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExamModel model)
        {
            ViewData["msg"] = "";
            if (string.IsNullOrEmpty(model.ExamName))
            {
                return View(model);
            }
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var allExam = await _examService.Create(
                model,
                Int32.Parse(User.FindFirst("UserID").Value),
                accessToken);
            if (allExam!=null && allExam.msg != null)
            {
                ViewData["msg"] = allExam.msg;
                return View();
            }
            else if(allExam==null)
            {
                ViewData["msg"] = "Error";
                return View();
            }
            else
            {
                ViewData["msg"] = "Thêm mới thành công";
                return View();
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["msg"] = "";
            try
            {
                string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var res = await _examService.GetByID(id, accessToken);
                if (res.data == null || res.msg != null)
                {
                    ViewData["msg"] = res.msg;
                }
                var exam = res.data;
                var model = new ExamModel()
                {
                    ID = exam.ID,
                    ExamName = exam.ExamName,
                    ImageURL = exam.ImageURL,
                    CategoryID = exam.CategoryID,
                    isActive = exam.isActive,
                    isPrivate = exam.isPrivate,
                    Time = exam.Time
                };
                return View(model);
            }
            catch (Exception)
            {
                ViewData["msg"] = "Lỗi hệ thống. Thử lại sau.";
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ExamModel model)
        {
            ViewData["msg"] = "";
            try
            {
                if (id != model.ID)
                {
                    ViewData["msg"] = "Không hợp lệ";
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.ExamName))
                {
                    ViewData["msg"] = "Không được bỏ trống";
                    return View(model);
                }
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var newExam = await _examService.Update(id, model, token);
                if (newExam.msg !=null)
                {
                    ViewData["msg"] = newExam.msg;
                    return View();
                }
                ViewData["msg"] = "Cập nhật thành công";
                return View();
            }
            catch (Exception)
            {
                ViewData["msg"] = "Lỗi hệ thống. Thử lại sau.";
                return View();
            }
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
                DeleteManyModel<int> temp = new DeleteManyModel<int>();
                temp.ListItem = new List<int>();
                temp.ListItem.AddRange(s);
                var result = await _examService.DeleteMany(temp, token);
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
        public async Task<IActionResult> ShowQuestions(int id, string keyword, int pageIndex = 1, int pageSize = 10)
        {
            ViewData["SubTitle"] = "";
            ViewData["msg"] = "";
            ViewData["examID"] = id;
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var examName = await _examService.GetByID(id, token);
                ViewData["SubTitle"] = examName!=null?"Đề thi: "+examName.data.ExamName:"";
                var model = new QuestionPagingRequest()
                {
                    ExamID = id,
                    keyword = keyword,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var allQuestions = await _questionManage.GetAllPaging(model, token);
                if (allQuestions.msg != null)
                {
                    ViewData["msg"] = allQuestions.msg;
                    return View();
                }
                if(allQuestions.data.Items!=null)
                    allQuestions.data.Items = allQuestions.data.Items.Where(q => q.ExamID == id).ToList();
                return View(allQuestions.data);
            }
            catch (Exception)
            {
                ViewData["msg"] = "Lỗi hệ thống. Vui lòng đăng nhập lại.";
                return View();
            }
        }
    }
}
