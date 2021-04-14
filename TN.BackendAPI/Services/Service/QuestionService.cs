using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class QuestionService : IQuestionService
    {
        private readonly TNDbContext _db;
        public QuestionService(TNDbContext db)
        {
            _db = db;
        }

        public async Task<ResponseBase<Question>> Create(QuestionModel model)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true && e.ID == model.ExamID).Include(e=>e.Questions).FirstAsync();
            if (exam == null)
            {
                return new ResponseBase<Question>()
                {
                    success = false,
                    msg = "Invalid exam"
                };
            }
            var newQuestion = new Question()
            {
                QuesContent = model.QuesContent,
                Option1 = model.Option1,
                Option2 = model.Option2,
                Option3 = model.Option3,
                Option4 = model.Option4,
                Answer = model.Answer,
                isActive = true,
                ExamID = model.ExamID,
                ImgURL = model.ImgURL,
                Exam = exam
            };
            if(exam.Questions == null)
            {
                newQuestion.STT = 1;
            }
            else
            {
                newQuestion.STT = exam.Questions.Count + 1;
            }
            try
            {
                _db.Questions.Add(newQuestion);
                await _db.SaveChangesAsync();
                return new ResponseBase<Question>()
                {
                    success = true,
                    msg = "Created successfully",
                    data = newQuestion
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<Question>()
                {
                    success = false,
                    msg = e.Message
                };
            }

        }

        public async Task<ResponseBase<bool>> Delete(int id)
        {
            var question = await _db.Questions.Where(q => q.isActive == true).FirstOrDefaultAsync();
            try
            {
                if (question != null)
                {
                    question.Results = null;
                    question.isActive = false;
                    await _db.SaveChangesAsync();
                    return new ResponseBase<bool>()
                    {
                        success = true,
                        data = true,
                        msg = "Deleted successfully"
                    };
                }
                else
                {
                    return new ResponseBase<bool>()
                    {
                        success = false,
                        data = false,
                        msg = "Not found"
                    };
                }
            }
            catch (Exception e)
            {

                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = e.Message
                };
            }
        }

        public async Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstId)
        {
            try
            {
                foreach (var id in lstId.ListItem)
                {
                    var q = await _db.Questions.FindAsync(id);
                    if (q.Results != null)
                    {
                        foreach (var record in q.Results)
                        {
                            _db.Results.Remove(record);
                        }
                    }
                    _db.Questions.Remove(q);
                }
                await _db.SaveChangesAsync();
                return new ResponseBase<bool>()
                {
                    success = true,
                    data = true,
                    msg = "Deleted successfully"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<List<Question>>> GetAll()
        {
            var lstQuestion = await _db.Questions.Where(q => q.isActive == true && q.Exam.isActive == true).ToListAsync();
            return new ResponseBase<List<Question>>() 
            {
                data = lstQuestion,
                success = true
            };
        }
        public async Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model)
        {
            var allQuestions = await _db.Questions.Where(q => q.isActive == true && q.Exam.isActive == true).Include(q => q.Results).Include(q => q.Exam).ToListAsync();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allQuestions = allQuestions.Where(q => q.QuesContent.Contains(model.keyword) ||
                q.Exam.ExamName.Contains(model.keyword)
                ).ToList();
            }
            if (model.ExamID > 0)
            {
                allQuestions = allQuestions.Where(q => q.ExamID == model.ExamID).ToList();
            }
            // get total row from query
            int totalRecords = allQuestions.Count;
            // get so trang
            int totalPages = 0;
            if(totalRecords > model.PageSize)
            {
                if(totalRecords % model.PageSize == 0)
                {
                    totalPages = totalRecords / model.PageSize;
                }
                else
                {
                    totalPages = totalRecords / model.PageSize + 1;
                }
            }
            // get data and paging
            var data = allQuestions.Skip((model.PageIndex - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(q => new Question()
                {
                    ID = q.ID,
                    QuesContent = q.QuesContent,
                    Answer = q.Answer,
                    ExamID = q.ExamID,
                    ImgURL = q.ImgURL,
                    isActive = q.isActive,
                    Option1 = q.Option1,
                    Option2 = q.Option2,
                    Option3 = q.Option3,
                    Option4 = q.Option4,
                    STT = q.STT,
                    Exam = q.Exam,
                    Results = q.Results
                })
                .ToList();
            // return
            return new ResponseBase<PagedResult<Question>>()
            {
                data = new PagedResult<Question>()
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

        public async Task<ResponseBase<Question>> GetOne(int id)
        {
            var question = await _db.Questions.Where(q => q.isActive == true && q.ID == id && q.Exam.isActive == true).Include(q => q.Exam).Include(q => q.Results).FirstOrDefaultAsync();
            if (question == null)
            {
                return new ResponseBase<Question>()
                {
                    success = false,
                    msg = "Not found"
                };
            }
            return new ResponseBase<Question>()
            {
                success = true,
                data = question
            };
        }

        public async Task<ResponseBase<Question>> Update(QuestionModel model)
        {
            var question = await _db.Questions.Where(q => q.isActive == true && q.ID == model.ID).FirstOrDefaultAsync();
            if(question != null)
            {
                if (string.IsNullOrEmpty(question.QuesContent))
                {
                    return new ResponseBase<Question>()
                    {
                        success = false,
                        msg = "Content can not be left blank"
                    };
                }
                question.QuesContent = model.QuesContent;
                question.Option1 = model.Option1;
                question.Option2 = model.Option2;
                question.Option3 = model.Option3;
                question.Option4 = model.Option4;
                question.Answer = model.Answer;
                question.ImgURL = model.ImgURL;
                try
                {
                    await _db.SaveChangesAsync();
                    return new ResponseBase<Question>()
                    {
                        success = true,
                        msg = "Updated successfully",
                        data = question
                    };
                }
                catch (Exception e)
                {
                    return new ResponseBase<Question>()
                    {
                        success = false,
                        msg = e.Message
                    };
                }
            }
            return new ResponseBase<Question>()
            {
                success = false,
                msg = "Not found"
            };
        }
    }
}
