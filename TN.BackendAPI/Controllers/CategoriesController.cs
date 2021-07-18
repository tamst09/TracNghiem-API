using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        private readonly IExamAdminService _examService;

        public CategoriesController(ICategoryService categoryService, IExamAdminService examService)
        {
            _categoryService = categoryService;
            _examService = examService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(new ResponseBase<List<Category>>() { data = categories });
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetByID(id);
            if (category != null)
            {
                return Ok(new ResponseBase<Category>() { data = category });
            }
            return Ok(new ResponseBase(msg: "Not found", success: false));
        }

        // GET: api/Categories/Exams/5
        [HttpGet("Exams/{id}")]
        public async Task<IActionResult> GetExams(int id)
        {
            var exams = await _examService.GetByCategory(id);
            if (exams != null)
            {
                return Ok(new ResponseBase<List<Exam>>() { data = exams });
            }
            else
            {
                return Ok(new ResponseBase(msg: "Exam not available", success: false));
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> CountCategory()
        {
            return Ok(new ResponseBase<int>(data: await _categoryService.CountCategory()));
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.ID)
            {
                return Ok(new ResponseBase(success: false, msg: "Category not fount"));
            }
            var updateResult = await _categoryService.Update(category);
            if (updateResult == null)
            {
                return Ok(new ResponseBase(success: false, msg: "Update failed"));
            }
            return Ok(new ResponseBase<Category>() { data = updateResult });
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Add([Bind("CategoryName")] Category category)
        {
            var createResult = await _categoryService.Create(category);
            return Ok(new ResponseBase<Category>() { data = createResult });
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
                return Ok(new ResponseBase(msg: "Delete failed"));
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
                return Ok(new ResponseBase(msg: "Delete failed"));
        }
    }
}
