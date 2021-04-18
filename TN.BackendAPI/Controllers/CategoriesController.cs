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
            return Ok(await _categoryService.GetAll());
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            return Ok(await _categoryService.GetOne(id));
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
            return Ok(await _categoryService.Update(category));
        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostCategory([Bind("CategoryName")] Category category)
        {
            return Ok(await _categoryService.Create(category));
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            return Ok(await _categoryService.Delete(id));
        }

        // POST: api/Categories/DeleteMany
        [HttpPost("DeleteMany")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteManyCategory(DeleteRangeModel<int> lstCategoryId)
        {
            return Ok(await _categoryService.DeleteMany(lstCategoryId));
        }

        // GET: api/Categories/Exams/5
        [HttpGet("Exams/{id}")]
        public async Task<IActionResult> GetExams(int id)
        {
            return Ok(await _categoryService.GetOne(id));
        }
    }
}
