using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IExamService
    {
        // ADMIN
        Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model);
        Task<ResponseBase<Exam>> GetOne(int id);
        Task<ResponseBase<Exam>> Update(ExamModel request);
        Task<ResponseBase<bool>> Delete(int examID);
        Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstExamId);

        //USER
        Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model, int userID);
        Task<ResponseBase<PagedResult<Exam>>> GetOwnedPaging(ExamPagingRequest model, int userID);
        Task<ResponseBase<List<Exam>>> GetOwned(int userID);
        Task<ResponseBase<Exam>> GetOne(int id, int userID);
        Task<ResponseBase<Exam>> Update(ExamModel request, int userID);
        Task<ResponseBase<bool>> Delete(int examID, int userID);

        //COMMON
        Task<ResponseBase<Exam>> Create(ExamModel request, int userID);
        Task<ResponseBase<int>> IncreaseAttemps(int examID);
    }
}
