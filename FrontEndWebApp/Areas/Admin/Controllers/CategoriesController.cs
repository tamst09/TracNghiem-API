using FrontEndWebApp.Areas.Admin.AdminServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class CategoriesController : Controller
    {
        public ICategoryManage _categoryService;
        public IExamManage _examService;

        public CategoriesController(ICategoryManage categoryService, IExamManage examService)
        {
            _categoryService = categoryService;
            _examService = examService;
        }

        public async Task<ActionResult> Index()
        {
            var getAllCategoriesRes = await _categoryService.GetAll();
            return View(getAllCategoriesRes.data);
        }

        public ActionResult Create()
        {
            ViewData["msg"] = "";
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Category model)
        {
            ViewData["msg"] = "";
            if (string.IsNullOrEmpty(model.CategoryName))
            {
                ViewData["msg"] = "Tên chủ đề không được bỏ trống";
                return View(model);
            }

            var result = await _categoryService.Create(model);
            ViewData["msg"] = result.msg;
            if (result.success)
                return View();
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            ViewData["msg"] = "";
            var result = await _categoryService.GetByID(id);
            if (result.success)
            {
                ViewData["msg"] = result.msg;
                return View();
            }
            return View(result.data);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Category model)
        {
            ViewData["msg"] = "";
            if (string.IsNullOrEmpty(model.CategoryName))
            {
                ViewData["msg"] = "Tên chủ đề không được bỏ trống";
                return View(model);
            }
            var result = await _categoryService.Update(model);
            if (result.success)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _categoryService.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> DeleteRange([FromBody] int[] s)
        {
            if (s.Length == 0)
            {
                return Json(new { deleteResult = false });
            }
            DeleteManyModel<int> temp = new DeleteManyModel<int>();
            temp.ListItem.AddRange(s);
            var result = await _categoryService.DeleteRange(temp);
            return Json(new { deleteResult = result.success });
        }

        [HttpGet]
        public async Task<IActionResult> ViewAllExams(int categoryID)
        {
            ViewData["CategoryName"] = _categoryService.GetByID(categoryID).Result.data.CategoryName;

            var getExamsResponse = await _examService.GetByCategory(categoryID);
            if (!getExamsResponse.success)
            {
                ViewData["msg"] = getExamsResponse.msg;
                return View();
            }
            return View(getExamsResponse.data);
        }
    }
}
