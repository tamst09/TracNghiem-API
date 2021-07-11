using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IExamUserService
    {
        Task<List<Exam>> GetAll(int userID);
        Task<List<Exam>> GetOwned(int userID);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, int userID);
        Task<PagedResult<Exam>> GetOwnedPaging(ExamPagingRequest model, int userID);
        
        Task<Exam> GetByID(int id, int userID);
        Task<bool> Update(ExamModel model, int userID);
        Task<bool> Delete(int examID, int userID);
        Task<bool> DeleteMany(DeleteManyModel<int> lstExamId);

        Task<Exam> Create(ExamModel request, int userID);
        Task<int> IncreaseAttemps(int examID);
    }
}
