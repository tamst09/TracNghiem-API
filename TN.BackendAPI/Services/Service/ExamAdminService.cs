using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class ExamAdminService : IExamAdminService
    {
        private readonly TNDbContext _db;
        public ExamAdminService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Delete(int examId)
        {
            var exam = await _db.Exams.FindAsync(examId);
            if (exam == null) return false;
            if (exam.Questions != null)
            {
                foreach (var q in exam.Questions)
                {
                    q.isActive = false;
                }
            }
            exam.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMany(DeleteManyModel<int> lstExamId)
        {
            try
            {
                List<Exam> lstExam = new List<Exam>();
                foreach (var item in lstExamId.ListItem)
                {
                    var e = await _db.Exams.Where(e => e.ID == item).Include(e => e.Questions).FirstOrDefaultAsync();
                    if (e.Questions != null)
                    {
                        foreach (var q in e.Questions)
                        {
                            q.isActive = false;
                        }
                    }
                    e.isActive = false;
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Exam>> GetAll()
        {
            var list = await _db.Exams.Where(e => e.isActive == true && e.Owner.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
            list.OrderBy(e => e.ExamName).ToList();
            return list;
        }

        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model)
        {
            var allExams =  _db.Exams
                .Where(e => e.isActive == true && e.Owner.isActive == true);
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (model.CategoryID > 0)
            {
                allExams = allExams.Where(e => e.CategoryID == model.CategoryID);
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExams = allExams
                    .Where(e => e.ExamName.ToLower().Contains(model.keyword.ToLower()) || e.Owner.UserName.ToLower().Contains(model.keyword.ToLower()));
            }
            // get total row from query
            int totalrecord = allExams.Count();
            // get so trang
            int pageCount = 0;
            if (totalrecord > model.PageSize)
            {
                if (totalrecord % model.PageSize == 0)
                {
                    pageCount = totalrecord / model.PageSize;
                }
                else
                {
                    pageCount = totalrecord / model.PageSize + 1;
                }
            }
            // get data and paging
            var data = await allExams
                .OrderBy(e => e.ID)
                .Skip((model.PageIndex - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(u => new Exam()
                {
                    ID = u.ID,
                    CategoryID = u.CategoryID,
                    ExamName = u.ExamName,
                    ImageURL = u.ImageURL,
                    NumOfAttemps = u.NumOfAttemps,
                    OwnerID = u.OwnerID,
                    Time = u.Time,
                    TimeCreated = u.TimeCreated,
                    isActive = u.isActive,
                    isPrivate = u.isPrivate,
                    Category = u.Category,
                    Owner = u.Owner,
                    Questions = u.Questions
                })
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .Include(e => e.Questions)
                .ToListAsync();
            // return
            return new PagedResult<Exam>()
            {
                Items = data,
                TotalRecords = totalrecord,
                TotalPages = pageCount,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            };
        }

        public async Task<List<Exam>> GetByCategory(int categoryId)
        {
            var categories = await _db.Categories.Where(c => c.ID == categoryId && c.isActive == true).ToListAsync();
            if (categories != null)
            {
                var exams = _db.Exams
                    .Where(e => e.CategoryID == categoryId && e.isActive == true)
                    .Include(e => e.Category)
                    .Include(e => e.Owner)
                    .Include(e => e.Questions)
                    .ToList();
                return exams;
            }
            else
            {
                return new List<Exam>();
            }
        }

        public async Task<Exam> GetByID(int id)
        {
            var exam = await _db.Exams
                .Where(e => e.isActive == true)
                .Include(e => e.Owner)
                .Include(e => e.Questions)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                return null;
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }

        public async Task<bool> Update(ExamModel request)
        {
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return false;
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            exam.CategoryID = request.CategoryID;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<int> Count()
        {
            var numberExam = await _db.Exams.CountAsync();
            return numberExam;
        }
    }
}
