using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        public ICategoryManage _categoryService;

        public CategoriesController(ICategoryManage categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<ActionResult> Index()
        {
            try
            {
                var categories = await _categoryService.GetAll();
                return View(categories.data);
            }
            catch (Exception)
            {
                return View();
            }
        }

        public ActionResult Create()
        {
            ViewData["Avatar"] = HttpContext.Session.GetString("avatarURL");
            ViewData["msg"] = "";
            try
            {
                return View();
            }
            catch (Exception)
            {
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Create(Category model)
        {
            ViewData["msg"] = "";
            try
            {
                if (string.IsNullOrEmpty(model.CategoryName))
                {
                    ViewData["msg"] = "Tên chủ đề không được bỏ trống";
                    return View(model);
                }
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var result = await _categoryService.Create(model, token);
                if (result.msg!=null)
                {
                    ViewData["msg"] = result.msg;
                    return View(model);
                }
                else if (result.data != null)
                {
                    ViewData["msg"] = "Tạo mới thành công";
                    return View(model);
                }
                else
                {
                    ViewData["msg"] = "Lỗi không xác định";
                    return View(model);
                }
            }
            catch (Exception)
            {
                ViewData["msg"] = "Something went wrong. Try again.";
                return View(model);
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            ViewData["msg"] = "";
            try
            {
                var model = await _categoryService.GetByID(id);
                return View(model.data);
            }
            catch (Exception)
            {
                ViewData["msg"] = "Something went wrong. Try again.";
                return View();
            }
        }
        [HttpPost]
        public async Task<ActionResult> Edit(int id, Category model)
        {
            ViewData["msg"] = "";
            try
            {
                if (id != model.ID)
                {
                    ViewData["msg"] = "Không hợp lệ";
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.CategoryName))
                {
                    ViewData["msg"] = "Tên chủ đề không được bỏ trống";
                    return View(model);
                }
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                await _categoryService.Update(id, model, token);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var result = await _categoryService.Delete(id, token);
                return RedirectToAction("Index");
                //return Json(new { deleteResult = result });
            }
            catch
            {
                return RedirectToAction("Index");
                //return Json(new { deleteResult = false });
            }
        }
        [HttpPost]
        public async Task<ActionResult> DeleteRange([FromBody]int[] s)
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
                var result = await _categoryService.DeleteRange(temp, token);
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

        [HttpGet]
        public async Task<IActionResult> ViewAllExams(int categoryID)
        {
            ViewData["CategoryName"] = _categoryService.GetByID(categoryID).Result.data.CategoryName;
            var accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var lstExams = await _categoryService.GetAllExams(categoryID, accessToken);
            if(lstExams == null)
            {
                ViewData["msg"] = "Lỗi kết nối máy chủ";
                return View();
            }
            if(lstExams.success == false)
            {
                return RedirectToAction("Login", "Account", new { Area =""});
            }
            if(lstExams.msg != null || lstExams.data == null)
            {
                ViewData["msg"] = lstExams.msg;
                return View();
            }
            return View(lstExams.data);
        }
    }
}
