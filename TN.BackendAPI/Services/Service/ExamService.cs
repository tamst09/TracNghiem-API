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
        public async Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model)
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
            int totalRecords = allExam.Count;
            // get so trang
            int totalPages = 0;
            if (totalRecords > model.PageSize)
            {
                if (totalRecords % model.PageSize == 0)
                {
                    totalPages = totalRecords / model.PageSize;
                }
                else
                {
                    totalPages = totalRecords / model.PageSize + 1;
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
            return new ResponseBase<PagedResult<Exam>>()
            {
                data = new PagedResult<Exam>()
                {
                    Items = data,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                },
                msg = "",
                success = true
            };
        }
        public async Task<ResponseBase<Exam>> GetOne(int id)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
            {
                return new ResponseBase<Exam>()
                {
                    data = null,
                    success = false,
                    msg = "Exam not found"
                };
            }
            exam.Questions.OrderBy(e => e.STT).ToList();
            return new ResponseBase<Exam>()
            {
                data = exam,
                success = true,
                msg = "An exam found"
            };
        }
        public async Task<ResponseBase<Exam>> Update(ExamModel request)
        {
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return new ResponseBase<Exam>()
            {
                data = null,
                success = false,
                msg = "Update failed"
            };
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            exam.CategoryID = request.CategoryID;
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<Exam>()
                {
                    data = exam,
                    success = true,
                    msg = "Updated"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<Exam>()
                {
                    data = null,
                    success = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<bool>> Delete(int examID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null) return new ResponseBase<bool>()
            {
                success = false,
                msg = "Exam not found"
            };
            if (exam.Questions != null)
            {
                foreach (var q in exam.Questions)
                {
                    q.isActive = false;
                }
            }
            exam.isActive = false;
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<bool>()
                {
                    success = true,
                    msg = "Deleted"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstExamId)
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
                return new ResponseBase<bool>()
                {
                    success = true,
                    msg = "Deleted"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    msg = e.Message
                };
            }
        }
        //===================================================================================================

        //======================================== USER REGION ==============================================
        public async Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model, int userID)
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
            int totalRecords = allExam.Count;
            // get so trang
            int totalPages = 0;
            if (totalRecords > model.PageSize)
            {
                if (totalRecords % model.PageSize == 0)
                {
                    totalPages = totalRecords / model.PageSize;
                }
                else
                {
                    totalPages = totalRecords / model.PageSize + 1;
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
            return new ResponseBase<PagedResult<Exam>>()
            {
                data = new PagedResult<Exam>()
                {
                    Items = data,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                },
                msg = "",
                success = true
            };
        }
        public async Task<ResponseBase<PagedResult<Exam>>> GetOwnedPaging(ExamPagingRequest model, int userID)
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
            int totalRecords = allExam.Count;
            // get so trang
            int totalPages = 0;
            if (totalRecords > model.PageSize)
            {
                if (totalRecords % model.PageSize == 0)
                {
                    totalPages = totalRecords / model.PageSize;
                }
                else
                {
                    totalPages = totalRecords / model.PageSize + 1;
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
            return new ResponseBase<PagedResult<Exam>>()
            {
                data = new PagedResult<Exam>()
                {
                    Items = data,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                },
                success = true
            };
        }
        public async Task<ResponseBase<List<Exam>>> GetOwned(int userID)
        {
            var allExam = await _db.Exams.Where(e => e.isActive == true).Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).ToListAsync();
            allExam = allExam.Where(e => e.OwnerID == userID).ToList();
            return new ResponseBase<List<Exam>>()
            {
                data = allExam,
                success = true
            };
        }
        public async Task<ResponseBase<Exam>> GetOne(int id, int userID)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true && (e.OwnerID == userID || (e.OwnerID!=userID && e.isPrivate==false))).Include(e => e.Owner).Include(e => e.Questions).Include(e => e.Category).FirstOrDefaultAsync(e => e.ID == id);
            if (exam == null)
            {
                return new ResponseBase<Exam>()
                {
                    success = false,
                    msg = "Exam not found"
                };
            }
            exam.Questions.OrderBy(e => e.STT).ToList();
            return new ResponseBase<Exam>()
            {
                success = true,
                msg = "An exam found",
                data = exam
            };
        }
        public async Task<ResponseBase<Exam>> Update(ExamModel request, int userID)
        {
            if (userID != request.OwnerID)
            {
                return new ResponseBase<Exam>()
                {
                    data = null,
                    success = false,
                    msg = "Invalid owner"
                };
            }
            var exam = await _db.Exams.FindAsync(request.ID);
            if (exam == null) return new ResponseBase<Exam>()
            {
                data = null,
                success = false,
                msg = "Update failed"
            };
            exam.ExamName = request.ExamName;
            exam.isPrivate = request.isPrivate;
            exam.Time = request.Time * 60;
            exam.CategoryID = request.CategoryID;
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<Exam>()
                {
                    data = exam,
                    success = true,
                    msg = "Updated"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<Exam>()
                {
                    data = null,
                    success = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<bool>> Delete(int examID, int userID)
        {
            var exam = await _db.Exams.FindAsync(examID);
            if (exam == null)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    msg = "Exam not found"
                };
            }
            if (exam.OwnerID != userID)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    msg = "Invalid owner"
                };
            }
            try
            {
                if (exam.Questions != null)
                {
                    foreach (var q in exam.Questions)
                    {
                        _db.Questions.Remove(q);
                    }
                }
                exam.isActive = false;
                await _db.SaveChangesAsync();
                return new ResponseBase<bool>()
                {
                    success = true,
                    msg = "Deleted"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    msg = e.Message
                };
            }
        }
        //===================================================================================================

        //======================================== COMMON REGION ============================================
        public async Task<ResponseBase<Exam>> Create(ExamModel request, int userID)
        {
            var owner = _db.Users.Where(u => u.Id == userID).FirstOrDefault();
            if(owner==null || owner.isActive == false)
            {
                return new ResponseBase<Exam>()
                {
                    success = false,
                    msg = "Invalid owner"
                };
            }
            try
            {
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
                return new ResponseBase<Exam>()
                {
                    success = true,
                    data = exam,
                    msg = "Created successfully"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<Exam>()
                {
                    success = true,
                    msg = e.Message
                };
            }
            
            
        }
        public async Task<ResponseBase<int>> IncreaseAttemps(int examID)
        {
            try
            {
                var exam = await _db.Exams.FindAsync(examID);
                exam.NumOfAttemps += 1;
                await _db.SaveChangesAsync();
                return new ResponseBase<int>()
                {
                    data = exam.NumOfAttemps,
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<int>()
                {
                    success = false,
                    msg = e.Message
                };
            }
            
        }
        //===================================================================================================
    }
}
