using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    // api/Categories
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var allCategory = await _categoryService.GetAll();
            return Ok(allCategory);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetOne(id);
            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.ID)
            {
                return BadRequest();
            }
            var updateResult = await _categoryService.Update(category);
            return Ok(updateResult);
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostCategory([Bind("CategoryName")] Category category)
        {
            var createResult = await _categoryService.Create(category);
            return Ok(createResult);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleteResult = await _categoryService.Delete(id);
            return Ok(deleteResult);
        }

        // POST: api/Categories/DeleteMany
        [HttpPost("DeleteMany")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteManyCategory(DeleteRangeModel<int> lstCategoryId)
        {
            var deleteResult = await _categoryService.DeleteMany(lstCategoryId);
            return Ok(deleteResult);
        }

        // GET: api/Categories/Exams/5
        [HttpGet("Exams/{id}")]
        public async Task<IActionResult> GetExams(int id)
        {
            var exams = await _categoryService.GetOne(id);
            return Ok(exams);
        }
    }
}
