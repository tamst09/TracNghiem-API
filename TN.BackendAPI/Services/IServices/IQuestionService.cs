using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IQuestionService
    {
        // ADMIN
        Task<List<Question>> GetAll();
        Task<PagedResult<Question>> GetAllPaging(QuestionPagingRequest model);
        Task<Question> GetByID(int id);
        Task<bool> Update(QuestionModel model);
        Task<bool> Delete(int id);
        Task<bool> DeleteMany(DeleteRangeModel<int> lstId);

        //USER
        Task<List<Question>> GetAllByExamID(int examID);

        //COMMON
        Task<Question> Create(QuestionModel model);
    }
}
