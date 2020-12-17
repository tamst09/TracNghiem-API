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
        public ExamService(TNDbContext db, IStorageService storageService)
        {
            _db = db;
        }
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
        public async Task<bool> Delete(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null) return false;
            exam.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<List<Exam>> GetAll()
        {
            var list = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
            list.OrderBy(e => e.ExamName).ToList();
            return list;
        }
        public async Task<PagedResultVM<Exam>> GetAllPaging(GetExamPagingRequest request)
        {
            var query = from e in _db.Exams
                        join c in _db.Categories on e.CategoryID equals c.ID
                        select e;
            if (!string.IsNullOrEmpty(request.keyword))
                query = query.Where(x => x.ExamName.Contains(request.keyword) || x.Owner.UserName.Contains(request.keyword));
            if (request.CategoryID.HasValue && request.CategoryID.Value > 0)
            {
                query = query.Where(e => e.CategoryID == request.CategoryID);
            }
            int totalrow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new Exam()
                {
                    ID = e.ID,
                    ExamName = e.ExamName,
                    TimeCreated = e.TimeCreated,
                    Time = e.Time,
                    isPrivate = e.isPrivate,
                    NumOfAttemps = e.NumOfAttemps,
                    ImageURL = e.ImageURL,
                    OwnerID = e.OwnerID,
                    CategoryID = (int)e.CategoryID,
                    isActive = e.isActive
                }).ToListAsync();
            var pageResult = new PagedResultVM<Exam>()
            {
                TotalRecord = totalrow,
                Items = data
            };
            return pageResult;
        }

        public async Task<Exam> GetByID(int id)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                throw new Exception("Exam not found");
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }
        public async Task<int> IncreaseAttemps(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            exam.NumOfAttemps += 1;
            return await _db.SaveChangesAsync();
        }

        public async Task<Exam> Update(Exam request)
        {
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return null;
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time;
            exam.CategoryID = request.CategoryID;
            exam.isActive = request.isActive;
            await _db.SaveChangesAsync();
            return exam;
        }

        //private async Task<string> SaveFile(IFormFile file)
        //{
        //    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
        //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
        //    await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
        //    return fileName;
        //}
    }
}
