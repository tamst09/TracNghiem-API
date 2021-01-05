using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.BackendAPI.Services.Service
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
            var findResult = _db.Categories.Where(c => c.CategoryName == request.CategoryName).FirstOrDefault();
            if (findResult!=null && !findResult.isActive)
            {
                findResult.Exams = null;
                findResult.isActive = true;
                return findResult;
            }
            var newCategory = new Category()
            {
                CategoryName = request.CategoryName,
                isActive = true,
                Exams = null
            };
            _db.Categories.Add(newCategory);
            await _db.SaveChangesAsync();
            return newCategory;
        }

        public async Task<Category> Update(Category request)
        {
            try
            {
                var findResult = _db.Categories.Where(c => c.ID == request.ID && c.isActive == true).FirstOrDefault();
                if (findResult != null)
                {
                    findResult.CategoryName = request.CategoryName;
                    await _db.SaveChangesAsync();
                    return findResult;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);
            if (category == null) return false;
            category.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetAll()
        {
            var categoryList = await _db.Categories.Where(c => c.isActive == true).ToListAsync();
            return categoryList;
        }

        public async Task<Category> GetByID(int id)
        {
            var category = await _db.Categories.Include(c => c.Exams).Where(c => c.isActive == true).FirstOrDefaultAsync(c => c.ID == id);
            if (category == null)
            {
                return null;
            }
            category.Exams = category.Exams.OrderBy(e => e.ExamName).ToList();
            return category;
        }
    }
}
