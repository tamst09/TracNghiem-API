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
        public async Task<List<Question>> GetAll()
        {
            return await _db.Questions.ToListAsync();
        }

        public Task<PagedResultVM<Question>> GetAllQuestionPaging(GetQuestionPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Question> GetByID(int questionID)
        {
            var question = await _db.Questions.Include(q => q.Exam).FirstOrDefaultAsync(q => q.ID == questionID);
            return question;
        }

        public async Task<List<Question>> GetListQuestionByExam(int examID)
        {
            var k = await _db.Exams.Include(e => e.Category).Include(e => e.Owner).Include(e => e.Questions).FirstOrDefaultAsync(e => e.ID == examID);
            if (k == null)
            {
                throw new Exception("Not found");
            }
            var result = k.Questions.ToList();
            return result;
        }

        public async Task<int> Create(Question request, int examID)
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
                STT = request.STT
            };
            //if (request.Image!=null)
            //    exam.ImageURL = await this.SaveFile(request.Image);
            _db.Questions.Add(question);
            return await _db.SaveChangesAsync();
        }
        public async Task<int> Update(Question request)
        {
            var question = await _db.Questions.FindAsync(request.ID);

            if (question == null) throw new Exception("question not found");
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
            return await _db.SaveChangesAsync();
        }
        public async Task<int> Delete(int questionID)
        {
            var question = await _db.Questions.FindAsync(questionID);
            if (question == null) throw new Exception("Exam not found");
            //await _storageService.DeleteFileAsync(exam.ImageURL);
            _db.Questions.Remove(question);
            return await _db.SaveChangesAsync();
        }
    }
}
