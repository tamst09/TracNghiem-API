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
        Task<ResponseBase<Question>> Create(QuestionModel model);
        Task<ResponseBase<bool>> Delete(int id);
        Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstId);
        Task<ResponseBase<List<Question>>> GetAll();
        Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model);
        Task<ResponseBase<Question>> GetOne(int id);
        Task<ResponseBase<Question>> Update(QuestionModel model);
    }
}
