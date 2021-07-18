using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Common;

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
                await _db.SaveChangesAsync();
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
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> Delete(int categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);
            if (category == null) return false;
            if (category.Exams != null)
            {
                foreach (var exam in category.Exams)
                {
                    exam.CategoryID = 0;
                    exam.Category = null;
                }
            }
            
            category.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMany(DeleteManyModel<int> lstCategoryId)
        {
            try
            {
                List<Category> lstCategory = new List<Category>();
                foreach (var cID in lstCategoryId.ListItem)
                {
                    var category = await _db.Categories.FindAsync(cID);
                    if (category.Exams != null)
                    {
                        foreach (var exam in category.Exams)
                        {
                            exam.isActive = false;
                        }
                    }
                    category.isActive = false;
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Category>> GetAll()
        {
            var categoryList = await _db.Categories.Where(c => c.isActive == true).ToListAsync();
            return categoryList;
        }

        public async Task<int> CountCategory()
        {
            return await _db.Categories.CountAsync();
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
