using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Implementor
{
    public class ManageExamService : IManageExamService
    {
        private readonly TNDbContext _db;
        private readonly IStorageService _storageService;
        public ManageExamService(TNDbContext db, IStorageService storageService)
        {
            _db = db;
            _storageService = storageService;

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

        public async Task<int> Delete(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null) throw new Exception("Exam not found");
            await _storageService.DeleteFileAsync(exam.ImageURL);
            _db.Exams.Remove(exam);
            return await _db.SaveChangesAsync();
        }

        public async Task<List<Exam>> GetAll()
        {
            var list = await _db.Exams.Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
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
                    CategoryID = (int)e.CategoryID
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
            var exam = await _db.Exams.Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                throw new Exception("Exam not found");
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }

        public async Task IncreaseAttemps(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            exam.NumOfAttemps += 1;
            await _db.SaveChangesAsync();
        }

        public async Task<int> Update(Exam request)
        {
            var exam = await _db.Exams.FindAsync(request.ID);

            if (exam == null) throw new Exception("exam not found");
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            //if(request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            exam.CategoryID = request.CategoryID;
            return await _db.SaveChangesAsync();
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
