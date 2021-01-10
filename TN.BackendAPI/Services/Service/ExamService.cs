using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class ExamService : IExamService
    {
        private readonly TNDbContext _db;
        public ExamService(TNDbContext db)
        {
            _db = db;
        }

        //======================================== ADMIN REGION =============================================
        public async Task<List<Exam>> GetAll()
        {
            var list = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
            list.OrderBy(e => e.ExamName).ToList();
            return list;
        }
        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e => e.ExamName.Contains(model.keyword) ||
                e.Category.CategoryName.Contains(model.keyword) ||
                e.Owner.UserName.Contains(model.keyword)
                ).ToList();
            }
            // get total row from query
            int totalrecord = allExam.Count;
            // get so trang
            int soTrang = (totalrecord % model.PageSize == 0) ? (totalrecord / model.PageSize) : (totalrecord / model.PageSize + 1);
            // get data and paging
            var data = allExam.Skip((model.PageIndex - 1) * model.PageSize)
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
                .ToList();
            // return
            return new PagedResult<Exam>()
            {
                Items = data,
                TotalRecords = totalrecord,
                TotalPages = soTrang,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            };
        }
        public async Task<Exam> GetByID(int id)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                throw new Exception("Exam not found");
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }
        public async Task<bool> Update(Exam request)
        {
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return false;
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            exam.CategoryID = request.CategoryID;
            exam.isActive = request.isActive;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Delete(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null) return false;
            exam.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
        //===================================================================================================

        //======================================== USER REGION ==============================================
        public async Task<List<Exam>> GetAll(int userID)
        {
            var list = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
            list = list.Where(e => (e.isPrivate == false && e.OwnerID != userID) || (e.OwnerID == userID)).ToList();
            list.OrderBy(e => e.ExamName).ToList();
            return list;
        }
        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, int userID)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            allExam = allExam.Where(e => (e.isPrivate == false && e.OwnerID != userID) || (e.OwnerID == userID)).ToList();
            allExam.OrderBy(e => e.ExamName).ToList();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e => e.ExamName.Contains(model.keyword) ||
                e.Category.CategoryName.Contains(model.keyword) ||
                e.Owner.UserName.Contains(model.keyword)
                ).ToList();
            }
            // get total row from query
            int totalrecord = allExam.Count;
            // get so trang
            int soTrang = (totalrecord % model.PageSize == 0) ? (totalrecord / model.PageSize) : (totalrecord / model.PageSize + 1);
            // get data and paging
            var data = allExam.Skip((model.PageIndex - 1) * model.PageSize)
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
                .ToList();
            // return
            return new PagedResult<Exam>()
            {
                Items = data,
                TotalRecords = totalrecord,
                TotalPages = soTrang,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            };
        }
        public async Task<Exam> GetByID(int id, int userID)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true && e.OwnerID == userID).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                throw new Exception("Exam not found");
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }
        public async Task<bool> Update(Exam request, int userID)
        {
            if (userID != request.OwnerID)
            {
                return false;
            }
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return false;
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            exam.CategoryID = request.CategoryID;
            exam.isActive = request.isActive;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Delete(int examID, int userID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null) return false;
            if (exam.OwnerID != userID)
            {
                return false;
            }
            exam.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
        //===================================================================================================

        //======================================== COMMON REGION ============================================
        public async Task<Exam> Create(Exam request, int userID)
        {
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
            return exam;
        }
        public async Task<int> IncreaseAttemps(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            exam.NumOfAttemps += 1;
            return await _db.SaveChangesAsync();
        }
        //===================================================================================================
    }
}
