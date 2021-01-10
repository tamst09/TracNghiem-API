using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
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
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            return await _categoryService.GetAll();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _categoryService.GetByID(id);
            return Ok(category);
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.ID)
            {
                return Ok(null);
            }
            var updateResult = await _categoryService.Update(category);
            if (updateResult == null)
            {
                return Ok(null);
            }
            return Ok(updateResult);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([Bind("CategoryName")]Category category)
        {

            var createTask = await _categoryService.Create(category);
            return Ok(createTask);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var deleteResult = await _categoryService.Delete(id);
            if (deleteResult)
                return Ok("Success");
            else
                return Ok();
        }

        // DELETE: api/Categories/DeleteRange
        [HttpPost("DeleteRange")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteManyCategory(DeleteRangeModel<int> lstCategoryId)
        {
            var deleteResult = await _categoryService.DeleteListCategory(lstCategoryId);
            if (deleteResult)
                return Ok("Success");
            else
                return Ok();
        }
    }
}
