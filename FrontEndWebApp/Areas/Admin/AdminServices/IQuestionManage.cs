using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IQuestionManage
    {
        // ADMIN
        Task<ResponseBase<CountQuestionModel>> CountQuestion();
        Task<ResponseBase<List<Question>>> GetAll();
        Task<ResponseBase<PagedResult<Question>>> GetAllPaging(QuestionPagingRequest model);
        Task<ResponseBase<Question>> GetByID(int id);
        Task<ResponseBase> Update(QuestionModel model);
        Task<ResponseBase> Delete(int id);
        Task<ResponseBase> DeleteMany(DeleteManyModel<int> lstId);

        //USER
        Task<ResponseBase<List<Question>>> GetAllByExamID(int examID);

        //COMMON
        Task<ResponseBase> Create(QuestionModel model);
    }
}
