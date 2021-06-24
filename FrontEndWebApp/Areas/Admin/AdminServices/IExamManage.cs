using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IExamManage
    {
        Task<ResponseBase<List<Exam>>> GetAll(string accessToken);
        Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model, string accessToken);
        Task<ResponseBase<Exam>> GetByID(int id, string accessToken);
        Task<ResponseBase<Exam>> Update(int id, ExamModel model, string accessToken);
        Task<ResponseBase<Exam>> Delete(int id, string accessToken);
        Task<ResponseBase<string>> DeleteMany(DeleteManyModel<int> lstId, string accessToken);
        Task<ResponseBase<Exam>> Create(ExamModel model, int userID, string accessToken);
        Task<ResponseBase<int>> IncreaseAttemps(int id, string accessToken);
    }
}
