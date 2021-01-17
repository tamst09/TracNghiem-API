using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class UserExamService : IUserExamService
    {
        private readonly TNDbContext _db;
        public UserExamService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<int> Create(Exam request, int userID)
        {
            var exam = new Exam()
            {
                ExamName = request.ExamName,
                isPrivate = request.isPrivate,
                Time = request.Time * 60,
                TimeCreated = DateTime.Now,
                NumOfAttemps = 0,
                CategoryID = request.CategoryID,
                OwnerID = userID
            };
            //if (request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            _db.Exams.Add(exam);
            return await _db.SaveChangesAsync();
        }

        public async Task<int> Delete(int examID, int userID)
        {
            var lstExam = await _db.Exams.Where(e => e.OwnerID == userID).ToListAsync();
            var exam = lstExam.Where(e => e.ID == examID).FirstOrDefault();
            _db.Exams.Remove(exam);
            return await _db.SaveChangesAsync();
        }

        public async Task<List<Exam>> GetAll()
        {
            return await _db.Exams.ToListAsync();
        }

        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest request)
        {
            var query = from e in _db.Exams
                        join c in _db.Categories on e.CategoryID equals c.ID
                        select e;
            if (!string.IsNullOrEmpty(request.keyword))
                query = query.Where(x => x.ExamName.Contains(request.keyword) || x.Owner.UserName.Contains(request.keyword));
            if (request.CategoryID > 0)
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
                    Time = e.Time * 60,
                    isPrivate = e.isPrivate,
                    NumOfAttemps = e.NumOfAttemps,
                    ImageURL = e.ImageURL,
                    OwnerID = e.OwnerID,
                    CategoryID = (int)e.CategoryID
                }).ToListAsync();
            var pageResult = new PagedResult<Exam>()
            {
                TotalRecords = totalrow,
                Items = data
            };
            return pageResult;
        }

        public async Task<Exam> GetByID(int id)
        {
            var exam = await _db.Exams.FindAsync(id);
            if (exam == null)
                throw new Exception("Exam not found");
            return exam;
        }

        public async Task IncreaseAttemps(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            exam.NumOfAttemps += 1;
            await _db.SaveChangesAsync();
        }

        public async Task<int> Update(Exam request, int userID)
        {
            var listExam = await _db.Exams.Where(e => e.OwnerID == userID).ToListAsync();
            var exam = listExam.Where(e => e.ID == request.ID).FirstOrDefault();

            if (exam == null) throw new Exception("Exam not found");
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            //if(request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            exam.CategoryID = request.CategoryID;
            return await _db.SaveChangesAsync();
        }
    }
}
