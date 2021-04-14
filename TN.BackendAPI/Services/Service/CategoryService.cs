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
        public async Task<ResponseBase<Category>> Create(Category request)
        {
            var findResult = _db.Categories.Where(c => c.CategoryName == request.CategoryName).FirstOrDefault();
            if (findResult!=null && !findResult.isActive)
            {
                findResult.Exams = null;
                findResult.isActive = true;
                await _db.SaveChangesAsync();
                return new ResponseBase<Category>() { 
                    data = findResult,
                    msg = "Created successfully",
                    success = true
                };
            }
            var newCategory = new Category()
            {
                CategoryName = request.CategoryName,
                isActive = true,
                Exams = null
            };
            _db.Categories.Add(newCategory);
            await _db.SaveChangesAsync();
            return new ResponseBase<Category>()
            {
                data = newCategory,
                msg = "Created successfully",
                success = true
            };
        }

        public async Task<ResponseBase<Category>> Update(Category request)
        {
            try
            {
                var findResult = _db.Categories.Where(c => c.ID == request.ID && c.isActive == true).FirstOrDefault();
                if (findResult != null)
                {
                    findResult.CategoryName = request.CategoryName;
                    await _db.SaveChangesAsync();
                    return new ResponseBase<Category>()
                    {
                        data = findResult,
                        msg = "Created successfully",
                        success = true
                    };
                }
                return new ResponseBase<Category>()
                {
                    msg = "category id not found",
                    success = false
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<Category>()
                {
                    msg = "Exception: "+ e.Message,
                    success = false
                };
            }
        }

        public async Task<ResponseBase<bool>> Delete(int categoryID)
        {
            var category = await _db.Categories.FindAsync(categoryID);
            if (category == null)
            {
                return new ResponseBase<bool>()
                {
                    data = false,
                    msg = "category id not found",
                    success = false
                };
            }
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
            return new ResponseBase<bool>()
            {
                msg = "Deleted",
                success = true,
                data = true
            };
        }

        public async Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstCategoryId)
        {
            try
            {
                IEnumerable<Category> lstCategory = new List<Category>();
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
                return new ResponseBase<bool>()
                {
                    msg = "Deleted",
                    success = true,
                    data = true
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    msg = e.Message,
                    success = false,
                    data = false
                };
            }
        }

        public async Task<ResponseBase<List<Category>>> GetAll()
        {
            var categoryList = await _db.Categories.Where(c => c.isActive == true).ToListAsync();
            return new ResponseBase<List<Category>>()
            {
                msg = categoryList.Count+" category(s) found",
                success = true,
                data = categoryList
            };
        }

        public async Task<ResponseBase<Category>> GetOne(int id)
        {
            var category = await _db.Categories.Where(c => c.isActive == true).FirstOrDefaultAsync(c => c.ID == id);
            if (category == null)
            {
                return new ResponseBase<Category>()
                {
                    msg = "Category not found",
                    success = false
                };
            }
            category.Exams = category.Exams.OrderBy(e => e.ExamName).ToList();
            return new ResponseBase<Category>()
            {
                msg = "Category found",
                success = true,
                data = category
            };
        }
    }
}
