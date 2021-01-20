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
        Task<ResponseBase<Question>> GetByID(int id, string accessToken);
        Task<ResponseBase<List<Question>>> GetAllByExamID(int examID, string accessToken);
        Task<ResponseBase<PagedResult<Question>>> GetByExamPaging(QuestionPagingRequest model, string accessToken);

        // CRUD
        Task<ResponseBase<Question>> Create(QuestionModel model, string accessToken);
        Task<ResponseBase<string>> Update(QuestionModel model, string accessToken);
        Task<ResponseBase<string>> DeleteMany(DeleteRangeModel<int> lstId, string accessToken);
    }
}
