using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Catalog.Exams;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class ExamsController : Controller
    {
        private readonly IExamManage _examService;
        private readonly ICategoryManage _categoryService;
        private readonly IUserManage _userManage;

        public ExamsController(IExamManage examService, ICategoryManage categoryService, IUserManage userManage)
        {
            _examService = examService;
            _categoryService = categoryService;
            _userManage = userManage;
        }

        public async Task<IActionResult> Index()
        {
            string accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var allExam = await  _examService.GetAll(accessToken);
            if (allExam.msg != null)
            {
                ViewData["msg"] = allExam.msg;
                return View();
            }
            return View(allExam.data);   
        }

        public IActionResult Create()
        {
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
        public async Task<ActionResult> DeleteMany([FromBody] int[] s)
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

    }
}
