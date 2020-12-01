using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly TNDbContext _db;

        public CategoriesController(TNDbContext context)
        {
            _db = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<List<Category>>> GetCategories()
        {
            var categoryList = await _db.Categories.ToListAsync();
            return categoryList;
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _db.Categories.Include(c => c.Exams).FirstOrDefaultAsync(c => c.ID == id);
            if (category != null)
            {
                category.Exams = category.Exams.OrderBy(e => e.ExamName).ToList();
                return category;
            }
            return NotFound();
        }

        // PUT: api/Categories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.ID)
            {
                return BadRequest();
            }
            try
            {
                _db.Entry(category).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory([Bind("ID","CategoryName")]Category category)
        {
            _db.Categories.Add(new Category() { CategoryName = category.CategoryName });
            await _db.SaveChangesAsync();
            return Ok(category);
            //return CreatedAtAction("GetCategory", new { id = category.ID }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();

            return Ok();
        }

        private bool CategoryExists(int id)
        {
            return _db.Categories.Any(e => e.ID == id);
        }
    }
}
