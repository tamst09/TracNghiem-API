using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IExamAdminService _examAdminService;
        private readonly IExamUserService _examUserService;

        public CategoriesController(ICategoryService categoryService, IExamAdminService examAdminService, IExamUserService examUserService)
        {
            _categoryService = categoryService;
            _examAdminService = examAdminService;
            _examUserService = examUserService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(new ResponseBase<List<Category>>(data: categories));
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetByID(id);
            if (category != null)
            {
                return Ok(new ResponseBase<Category>(data: category));
            }
            return Ok(new ResponseBase(msg: "Not found.", success: false));
        }

        // GET: api/Categories/Exams/5
        [HttpGet("Exams/{id}")]
        public async Task<IActionResult> GetExams(int id)
        {
            var exams = new List<Exam>();
            if (User.IsInRole("admin"))
            {
                exams = await _examAdminService.GetByCategory(id);
                return Ok(new ResponseBase<List<Exam>>(data: exams));
            };
            exams = await _examUserService.GetByCategory(id, GetCurrentUserId());
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }

        [HttpGet("Count")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CountCategory()
        {
            return Ok(new ResponseBase<int>(data: await _categoryService.CountCategory()));
        }

        // PUT: api/Categories
        [HttpPut]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(Category category)
        {
            var updateResult = await _categoryService.Update(category);
            if (!updateResult)
            {
                return Ok(new ResponseBase(success: false, msg: "Update failed."));
            }
            return Ok(new ResponseBase());
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([Bind("CategoryName")] Category category)
        {
            var createResult = await _categoryService.Create(category);
            if (!createResult)
            {
                return Ok(new ResponseBase(success: false, msg: "Failed."));
            }
            return Ok(new ResponseBase());
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleteResult = await _categoryService.Delete(id);
            if (deleteResult)
                return Ok(new ResponseBase());
            else
                return Ok(new ResponseBase(msg: "Failed."));
        }

        // DELETE: api/Categories/DeleteRange
        [HttpPost("DeleteRange")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteMany(DeleteManyModel<int> lstCategoryId)
        {
            var deleteResult = await _categoryService.DeleteMany(lstCategoryId);
            if (deleteResult)
                return Ok(new ResponseBase());
            else
                return Ok(new ResponseBase(msg: "Failed."));
        }

        private int GetCurrentUserId()
        {
            var check = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (check)
                return userId;
            return -1;
        }
    }
}
