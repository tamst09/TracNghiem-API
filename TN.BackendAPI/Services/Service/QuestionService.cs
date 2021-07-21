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

        public async Task<bool> Create(QuestionModel model)
        {
            var exam = _db.Exams.Include(e => e.Questions).FirstOrDefault(e => e.isActive == true && e.ID == model.ExamID);
            if (exam == null)
            {
                return false;
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
                STT = model.STT
            };
            _db.Questions.Add(newQuestion);
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ResponseBase> AddListQuestions(AddListQuestionRequest request)
        {
            foreach (var question in request.Questions)
            {
                Question newQuestion = new Question()
                {
                    ExamID = question.ExamID,
                    Option1 = question.Option1,
                    Option2 = question.Option2,
                    Option3 = question.Option3,
                    Option4 = question.Option4,
                    Answer = question.Answer,
                    isActive = true,
                    QuesContent = question.QuesContent
                };
                _db.Questions.Add(newQuestion);
            }
            try
            {
                _db.SaveChanges();
                return new ResponseBase();
            }
            catch (Exception e)
            {
                return new ResponseBase(success: false, msg: e.Message);
            }
        }

        public async Task<bool> Delete(int id)
        {
            var question = await _db.Questions.FirstOrDefaultAsync(q => q.ID == id);
            if (question == null)
            {
                return false;
            }
            try
            {
                question.isActive = false;
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteMany(DeleteManyModel<int> lstId)
        {
            try
            {
                var questions = await _db.Questions.Where(q => lstId.ListItem.Contains(q.ID)).ToListAsync();
                foreach (var question in questions)
                {
                    question.isActive = false;
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

        public async Task<ResponseBase<List<Question>>> GetByExam(GetQuestionsByExamRequest request)
        {
            var exam = await _db.Exams.FirstOrDefaultAsync(e => e.isActive == true && e.ID == request.ExamId);
            if(exam == null)
            {
                return new ResponseBase<List<Question>>(success: false, msg: "Invalid exam ID.", data: null);
            }
            var questions = new List<Question>();
            questions = _db.Questions.Where(q => q.ExamID == request.ExamId && q.isActive == true).OrderBy(q => q.STT).ToList();
            return new ResponseBase<List<Question>>(data: questions);
        }

        public async Task<int> CountQuestions()
        {
            var countQuestions = await _db.Questions.Where(q => q.isActive == true).CountAsync();
            return countQuestions;
        }

        public async Task<PagedResult<Question>> GetAllPaging(QuestionPagingRequest model)
        {
            var allQuestions = _db.Questions.Where(q => q.isActive == true && q.Exam.isActive == true);
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allQuestions = allQuestions
                    .Where(q => q.QuesContent.Contains(model.keyword) || q.Exam.ExamName.Contains(model.keyword));
            }
            if (model.ExamID > 0)
            {
                allQuestions = allQuestions.Where(q => q.ExamID == model.ExamID);
            }
            // get total row from query
            int totalrecord = allQuestions.Count();
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
            var question = await _db.Questions
                .FirstOrDefaultAsync(q => q.isActive == true && q.ID == id && q.Exam.isActive == true);
            return question;
        }

        public async Task<bool> Update(QuestionModel model)
        {
            var question = await _db.Questions.FirstOrDefaultAsync(q => q.isActive == true && q.ID == model.ID);     
            if (question == null)
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
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
