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
            var list = await _db.Exams.Where(e => e.isActive == true && e.Owner.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).ToListAsync();
            list.OrderBy(e => e.ExamName).ToList();
            return list;
        }
        public async Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true && e.Owner.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (model.CategoryID > 0)
            {
                allExam = allExam.Where(e => e.CategoryID == model.CategoryID).ToList();
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e => e.ExamName.ToLower().Contains(model.keyword.ToLower()) ||
                e.Owner.UserName.ToLower().Contains(model.keyword.ToLower())
                ).ToList();
            }
            // get total row from query
            int totalrecord = allExam.Count;
            // get so trang
            int soTrang = 0;
            if (totalrecord > model.PageSize)
            {
                if (totalrecord % model.PageSize == 0)
                {
                    soTrang = totalrecord / model.PageSize;
                }
                else
                {
                    soTrang = totalrecord / model.PageSize + 1;
                }
            }
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
        public async Task<bool> Delete(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
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
                IEnumerable<Exam> lstExam = new List<Exam>();
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
            if (model.CategoryID > 0)
            {
                allExam = allExam.Where(e => e.CategoryID == model.CategoryID).ToList();
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e => e.ExamName.ToLower().Contains(model.keyword.ToLower())||
                e.Owner.UserName.ToLower().Contains(model.keyword.ToLower())
                ).ToList();
            }
            // get total row from query
            int totalrecord = allExam.Count;
            // get so trang
            int soTrang = 0;
            if (totalrecord > model.PageSize)
            {
                if (totalrecord % model.PageSize == 0)
                {
                    soTrang = totalrecord / model.PageSize;
                }
                else
                {
                    soTrang = totalrecord / model.PageSize + 1;
                }
            }
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
        public async Task<PagedResult<Exam>> GetOwnedPaging(ExamPagingRequest model, int userID)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            allExam = allExam.Where(e => e.OwnerID == userID).ToList();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (model.CategoryID > 0)
            {
                allExam = allExam.Where(e => e.CategoryID == model.CategoryID).ToList();
            }
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allExam = allExam.Where(e => e.ExamName.ToLower().Contains(model.keyword.ToLower()) ||
                e.Owner.UserName.ToLower().Contains(model.keyword.ToLower())
                ).ToList();
            }
            // get total row from query
            int totalrecord = allExam.Count;
            // get so trang
            int soTrang = 0;
            if (totalrecord > model.PageSize)
            {
                if (totalrecord % model.PageSize == 0)
                {
                    soTrang = totalrecord / model.PageSize;
                }
                else
                {
                    soTrang = totalrecord / model.PageSize + 1;
                }
            }
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
        public async Task<List<Exam>> GetOwned(int userID)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            allExam = allExam.Where(e => e.OwnerID == userID).ToList();
            return allExam;
        }
        public async Task<Exam> GetByID(int id, int userID)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true && (e.OwnerID == userID || (e.OwnerID!=userID && e.isPrivate==false))).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
                return null;
            exam.Questions.OrderBy(e => e.STT).ToList();
            return exam;
        }
        public async Task<List<Exam>> GetFavoritedExams(int userId)
        {
            List<Exam> exams = new List<Exam>();
            var favoritedExams = await _db.FavoriteExams.Where(e => e.UserID == userId).ToListAsync();
            foreach (var exam in favoritedExams)
            {
                var e = await _db.Exams.FindAsync(exam.ExamID);
                if (e != null)
                {
                    exams.Add(e);
                }
            }
            return exams;
        }
        public async Task<bool> AddFavoritedExam(int userId, int examId)
        {
            
            var favoriteExam = await _db.FavoriteExams.Where(e => e.ExamID == examId && e.UserID == userId).FirstOrDefaultAsync();
            if (favoriteExam == null)
            {
                var exam = await _db.Exams.FindAsync(examId);
                var user = await _db.Users.FindAsync(userId);
                if (exam != null && user != null)
                {
                    favoriteExam.AppUser = user;
                    favoriteExam.Exam = exam;
                    _db.FavoriteExams.Add(favoriteExam);
                    try
                    {
                        _db.SaveChanges();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return false;
                    }
                }
                else return false;
            }
            return true;
        }
        public async Task<bool> DeleteFavoritedExam(int userId, int examId)
        {
            var exam = await _db.FavoriteExams.Where(e => e.ExamID == examId && e.UserID == userId).FirstOrDefaultAsync();
            if (exam != null)
            {
                try
                {
                    _db.FavoriteExams.Remove(exam);
                    _db.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> Update(ExamModel request, int userID)
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
            if (exam.Questions != null)
            {
                foreach (var q in exam.Questions)
                {
                    _db.Questions.Remove(q);
                }
            }
            exam.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }
        //===================================================================================================

        //======================================== COMMON REGION ============================================
        public async Task<Exam> Create(ExamModel request, int userID)
        {
            var owner = _db.Users.Where(u => u.Id == userID).FirstOrDefault();
            if(owner==null || owner.isActive == false)
            {
                return null;
            }
            var category = _db.Categories.Where(c => c.ID == request.CategoryID).FirstOrDefault();
            var exam = new Exam()
            {
                ExamName = request.ExamName,
                isPrivate = request.isPrivate,
                Time = request.Time * 60,
                ImageURL = request.ImageURL,
                TimeCreated = DateTime.Now,
                NumOfAttemps = 0,
                CategoryID = request.CategoryID,
                Category = _db.Categories.Where(c => c.ID == request.CategoryID).FirstOrDefault(),
                OwnerID = userID,
                Owner = _db.Users.Where(u => u.Id == userID).FirstOrDefault(),
                Questions = null,
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
            await _db.SaveChangesAsync();
            return exam.NumOfAttemps;
        }
        //===================================================================================================
    }
}
