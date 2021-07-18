using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IExamAdminService
    {
        Task<List<Exam>> GetAll();
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model);
        Task<Exam> GetByID(int id);
        Task<List<Exam>> GetByCategory(int categoryId);
        Task<bool> Update(ExamModel request);
        Task<bool> Delete(int examId);
        Task<bool> DeleteMany(DeleteManyModel<int> lstExamId);
        Task<int> Count();
    }
}
