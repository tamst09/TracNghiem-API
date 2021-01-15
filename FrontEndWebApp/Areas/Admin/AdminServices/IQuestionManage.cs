using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IQuestionManage
    {
        // ADMIN
        Task<ResponseBase<List<Question>>> GetAll(string accessToken);
        Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model, string accessToken);
        Task<ResponseBase<Question>> GetByID(int id, string accessToken);
        Task<ResponseBase<string>> Update(QuestionModel model, string accessToken);
        Task<ResponseBase<string>> Delete(int id, string accessToken);
        Task<ResponseBase<string>> DeleteMany(DeleteRangeModel<int> lstId, string accessToken);

        //USER
        Task<ResponseBase<List<Question>>> GetAllByExamID(int examID, string accessToken);

        //COMMON
        Task<ResponseBase<Question>> Create(QuestionModel model, string accessToken);
    }
}
