using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Implementor
{
    public class QuestionService : IQuestionService
    {
        private readonly TNDbContext _db;
        public QuestionService(TNDbContext db)
        {
            _db = db;
        }

        public Task<PagedResult<Question>> GetAllQuestionPaging(GetQuestionPagingRequest request)
        {
            throw new NotImplementedException();
        }


        public async Task<List<Question>> GetListQuestionByExam(int examID)
        {
            var k = await _db.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.ID == examID);
            if (k == null)
            {
                return null;
            }
            var result = k.Questions.ToList();
            return result;
        }

        public async Task<Question> Create(Question request, int examID)
        {
            var question = new Question()
            {
                QuesContent = request.QuesContent,
                Option1 = request.Option1,
                Option2 = request.Option2,
                Option3 = request.Option3,
                Option4 = request.Option4,
                Answer = request.Answer,
                ImgURL = request.ImgURL,
                ExamID = examID,
                STT = request.STT,
                isActive = true
            };
            //if (request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            _db.Questions.Add(question);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<Question> Update(Question request)
        {
            var question = await _db.Questions.FindAsync(request.ID);

            if (question == null) return null;
            question.QuesContent = request.QuesContent;
            question.Option1 = request.Option1;
            question.Option2 = request.Option2;
            question.Option3 = request.Option3;
            question.Option4 = request.Option4;
            question.Answer = request.Answer;
            question.ImgURL = request.ImgURL;
            question.STT = request.STT;
            //if(request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<bool> Delete(int questionID)
        {
            var question = await _db.Questions.FindAsync(questionID);
            if (question == null) return false;
            if(question.isActive == true)
                question.isActive = false;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<Question> GetByID(int id)
        {
            var question = await _db.Questions.Where(e => e.isActive == true).Include(e => e.Exam).FirstOrDefaultAsync(e => e.ID == id);
            if (question == null)
                return null;
            return question;
        }
    }
}
