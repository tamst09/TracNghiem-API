using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IQuestionService
    {
        Task<List<Question>> GetAll();
        Task<PagedResult<Question>> GetAllPaging(QuestionPagingRequest model);
        Task<Question> GetByID(int id);
        Task<ResponseBase<List<Question>>> GetByExam(GetQuestionsByExamRequest request);
        Task<int> CountQuestions();
        Task<bool> Update(QuestionModel model);
        Task<bool> Delete(int id);
        Task<bool> DeleteMany(DeleteManyModel<int> lstId);
        Task<bool> Create(QuestionModel model);
    }
}
