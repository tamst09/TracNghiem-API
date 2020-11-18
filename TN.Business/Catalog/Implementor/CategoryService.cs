using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.Business.Catalog.Implementor
{
    public class CategoryService : ICategoryService
    {
        private readonly TNDbContext _db;
        public CategoryService(TNDbContext db)
        {
            _db = db;
        }
        public async Task<int> Create(Category request)
        {
            _db.Categories.Add(new Category() { CategoryName = request.CategoryName });
            return await _db.SaveChangesAsync();
        }

        public async Task<int> Delete(int categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);
            if (category == null) throw new Exception("Category not found");

            _db.Categories.Remove(category);
            return await _db.SaveChangesAsync();
        }

        public async Task<List<Category>> GetAll()
        {
            var categoryList = await _db.Categories.ToListAsync();
            return categoryList;
        }
        public async Task<int> Update(int id, Category request)
        {
            _db.Entry(request).State = EntityState.Modified;
            return await _db.SaveChangesAsync();
        }
        public async Task<Category> Get(int id)
        {
            var category = await _db.Categories.Include(c => c.Exams).FirstOrDefaultAsync(c => c.ID == id);
            category.Exams = category.Exams.OrderBy(e => e.ExamName).ToList();
            if (category != null)
            {
                return category;
            }
            return null;
        }
    }
}
