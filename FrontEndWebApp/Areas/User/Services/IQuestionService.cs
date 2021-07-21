using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IQuestionService
    {
        // GET
        Task<ResponseBase<Question>> GetByID(int id);
        Task<ResponseBase<List<Question>>> GetByExamID(int examID);
        Task<ResponseBase<PagedResult<Question>>> GetPagedQuestion(QuestionPagingRequest model);

        // CRUD
        Task<ResponseBase> Create(QuestionModel model);
        Task<ResponseBase> Update(QuestionModel model);
        Task<ResponseBase> DeleteMany(DeleteManyModel<int> lstId);
        Task<ResponseBase> Delete(int questionId);
    }
}
