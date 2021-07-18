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
    public class ExamUserService : IExamUserService
    {
        private readonly TNDbContext _db;

        public ExamUserService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(ExamModel request, int userID)
        {
            var owner = _db.Users.FirstOrDefault(u => u.Id == userID && u.isActive == true);
            if (owner == null)
            {
                return false;
            }
            var exam = new Exam()
            {
                ExamName = request.ExamName,
                isPrivate = request.isPrivate,
                Time = request.Time * 60,
                ImageURL = request.ImageURL,
                TimeCreated = DateTime.Now,
                NumOfAttemps = 0,
                CategoryID = request.CategoryID,
                OwnerID = userID,
                isActive = true
            };
            _db.Exams.Add(exam);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(int examID, int userID)
        {
            var exam = await _db.Exams.FirstOrDefaultAsync(e => e.OwnerID == userID && e.ID == examID);
            if (exam == null) return false;
            try
            {
                exam.isActive = false;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMany(DeleteManyModel<int> lstExamId)
        {
            try
            {
                var deleteExams = await _db.Exams.Where(e => lstExamId.ListItem.Contains(e.ID)).ToListAsync();
                foreach (var exam in deleteExams)
                {
                    exam.isActive = false;
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Exam>> GetAll(int userID)
        {
            var exams = await _db.Exams
                .Where(e => e.isActive == true && ((e.isPrivate == false && e.OwnerID != userID) || (e.OwnerID == userID)))
                .Include(e => e.Owner)
                .Include(e => e.Questions)
                .Include(e => e.Category)
                .OrderBy(e => e.ExamName)
                .ToListAsync();
            if (exams == null)
            {
                return new List<Exam>();
            }
            return exams;
        }

        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, int userID)
        {
            var allExam = _db.Exams
                .Where(e => e.isActive == true)
                .Where(e => (e.isPrivate == false && e.OwnerID != userID) || (e.OwnerID == userID));
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (model.CategoryID > 0)
            {
                allExam = allExam.Where(e => e.CategoryID == model.CategoryID);
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e =>
                    e.ExamName.ToLower().Contains(model.keyword.ToLower()) ||
                    e.Owner.UserName.ToLower().Contains(model.keyword.ToLower()));
            }
            allExam = allExam.OrderBy(e => e.ID);
            // get total row from query
            int totalrecord = allExam.Count();
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
            var data = await allExam.Skip((model.PageIndex - 1) * model.PageSize)
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

        public async Task<Exam> GetByID(int id, int userID)
        {
            var exam = await _db.Exams
                .Where(e => e.isActive == true && (e.OwnerID == userID || (e.OwnerID != userID && e.isPrivate == false)))
                .Include(e => e.Owner)
                .Include(e => e.Questions)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                return null;
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }

        public async Task<List<Exam>> GetOwned(int userID)
        {
            var exams = await _db.Exams
                .Where(e => e.OwnerID == userID && e.isActive == true)
                .Include(e => e.Category)
                .Include(e => e.Owner)
                .Include(e => e.Questions)
                .ToListAsync();
            return exams;
        }

        public async Task<PagedResult<Exam>> GetOwnedPaging(ExamPagingRequest model, int userID)
        {
            var allExams = _db.Exams
                .Where(e => e.isActive == true && e.OwnerID == userID);
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (model.CategoryID > 0)
            {
                allExams = allExams.Where(e => e.CategoryID == model.CategoryID);
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExams = allExams.Where(e =>
                    e.ExamName.ToLower().Contains(model.keyword.ToLower()) ||
                    e.Owner.UserName.ToLower().Contains(model.keyword.ToLower()));
            }
            allExams = allExams.OrderBy(e => e.ExamName);
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

        public async Task<int> IncreaseAttemps(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            exam.NumOfAttemps += 1;
            await _db.SaveChangesAsync();
            return exam.NumOfAttemps;
        }

        public async Task<bool> Update(ExamModel model, int userID)
        {
            var exam = _db.Exams.FirstOrDefault(e => e.isActive == true && e.OwnerID == userID && e.ID == model.ID);
            if (exam == null)
                return false;
            exam.ExamName = model.ExamName;
            exam.isPrivate = model.isPrivate;
            exam.Time = model.Time * 60;
            exam.CategoryID = model.CategoryID;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Exam>> GetByCategory(int categoryId, int userId)
        {
            var exams = _db.Exams.Where(e => e.Category.isActive == true && e.isActive == true && e.CategoryID == categoryId);
            exams = exams.Where(e => (e.OwnerID == userId) || (e.OwnerID != userId && e.isPrivate == false));
            var result = exams.Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions);
            return result.ToList();
        }
    }
}
