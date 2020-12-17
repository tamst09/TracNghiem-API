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
        public async Task<Category> Create(Category request)
        {
            _db.Categories.Add(new Category() 
            { 
                CategoryName = request.CategoryName,
                isAcive = true,
                Exams = null
            });
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<Category> Update(Category request)
        {
            try
            {
                _db.Entry(request).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return request;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);
            if (category == null) return false;
            category.isAcive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetAll()
        {
            var categoryList = await _db.Categories.Where(c => c.isAcive == true).ToListAsync();
            return categoryList;
        }

        public async Task<Category> GetByID(int id)
        {
            var category = await _db.Categories.Include(c => c.Exams).Where(c => c.isAcive == true).FirstOrDefaultAsync(c => c.ID == id);
            if (category == null)
            {
                return null;
            }
            category.Exams = category.Exams.OrderBy(e => e.ExamName).ToList();
            return category;
        }
    }
}
