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
        Task<List<Exam>> GetAll();
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model);
        Task<Exam> GetByID(int id);
        Task<bool> Update(ExamModel request);
        Task<bool> Delete(int examID);
        Task<bool> DeleteMany(DeleteManyModel<int> lstExamId);

        //USER
        Task<List<Exam>> GetAll(int userID);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, int userID);
        Task<PagedResult<Exam>> GetOwnedPaging(ExamPagingRequest model, int userID);
        Task<List<Exam>> GetOwned(int userID);
        Task<Exam> GetByID(int id, int userID);
        Task<bool> Update(ExamModel request, int userID);
        Task<bool> Delete(int examID, int userID);
        Task<List<Exam>> GetFavoritedExams(int userId);
        Task<bool> AddFavoritedExam(int userId, int examId);
        Task<bool> DeleteFavoritedExam(int userId, int examId);

        //COMMON
        Task<Exam> Create(ExamModel request, int userID);
        Task<int> IncreaseAttemps(int examID);
    }
}
