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

        public async Task<Question> Create(QuestionModel model)
        {
            var exam = await _db.Exams.Where(e => e.isActive == true && e.ID == model.ExamID).Include(e=>e.Questions).FirstAsync();
            if (exam == null)
            {
                return null;
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
            _db.Questions.Add(newQuestion);
            await _db.SaveChangesAsync();
            return newQuestion;
        }

        public async Task<bool> Delete(int id)
        {
            var question = await _db.Questions.Where(q => q.isActive == true).FirstOrDefaultAsync();
            if (question != null)
            {
                question.Results = null;
                question.isActive = false;
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteMany(DeleteManyModel<int> lstId)
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Question>> GetAll()
        {
            var lstQuestion = await _db.Questions.Where(q => q.isActive == true).ToListAsync();
            return lstQuestion;
        }

        public async Task<int> CountQuestions()
        {
            var countQuestions = await _db.Questions.Where(q => q.isActive == true).CountAsync();
            return countQuestions;
        }

        public async Task<List<Question>> GetByExam(int examID)
        {
            var exam = await _db.Exams.Where(e => e.ID == examID && e.isActive == true).FirstOrDefaultAsync();
            if (exam == null)
            {
                return null;
            }
            return exam.Questions;
        }

        public async Task<PagedResult<Question>> GetAllPaging(QuestionPagingRequest model)
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
            int totalrecord = allQuestions.Count;
            // get so trang
            int soTrang = 0;
            if(totalrecord > model.PageSize)
            {
                if(totalrecord % model.PageSize == 0)
                {
                    soTrang = totalrecord / model.PageSize;
                }
                else
                {
                    soTrang = totalrecord / model.PageSize + 1;
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
            return new PagedResult<Question>()
            {
                Items = data,
                TotalRecords = totalrecord,
                TotalPages = soTrang,
                PageIndex = model.PageIndex,
                PageSize = model.PageSize
            };
        }

        public async Task<Question> GetByID(int id)
        {
            var question = await _db.Questions.Where(q => q.isActive == true && q.ID == id && q.Exam.isActive == true).Include(q => q.Exam).Include(q => q.Results).FirstOrDefaultAsync();
            return question;
        }

        public async Task<bool> Update(QuestionModel model)
        {
            var question = await _db.Questions.Where(q => q.isActive == true && q.ID == model.ID).FirstOrDefaultAsync();
            if(question != null)
            {
                if (string.IsNullOrEmpty(question.QuesContent))
                {
                    return false;
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
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
    }
}
