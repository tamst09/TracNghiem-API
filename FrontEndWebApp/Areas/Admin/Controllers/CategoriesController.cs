using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;

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
                List<Category> categories = new List<Category>();
                categories = await _categoryService.GetAll();
                return View(categories);
            }
            catch (Exception)
            {
                return View(null);
            }
        }

        public ActionResult Create()
        {
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
                var token = Encoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var result = await _categoryService.Create(model, token);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                ViewData["msg"] = "Something went wrong. Try again.";
                return View(model);
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
                return View(model);
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
                var token = Encoder.DecodeToken(Request.Cookies["access_token_cookie"]);
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
                var token = Encoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var result = await _categoryService.Delete(id, token);
                return Json(new { deleteResult = result });
            }
            catch (Exception)
            {
                return Json(new { deleteResult = false });
            }
        }
    }
}
