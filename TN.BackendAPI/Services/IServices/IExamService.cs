using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IExamService
    {
        // ADMIN
        Task<List<Exam>> GetAll();
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model);
        Task<Exam> GetByID(int id);
        Task<bool> Update(ExamModel request);
        Task<bool> Delete(int examID);
        Task<bool> DeleteMany(DeleteRangeModel<int> lstExamId);

        //USER
        Task<List<Exam>> GetAll(int userID);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, int userID);
        Task<Exam> GetByID(int id, int userID);
        Task<bool> Update(ExamModel request, int userID);
        Task<bool> Delete(int examID, int userID);

        //COMMON
        Task<Exam> Create(ExamModel request, int userID);
        Task<int> IncreaseAttemps(int examID);
    }
}
