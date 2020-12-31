using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Interface
{
    public interface IExamService
    {
        Task<Exam> Create(Exam request, int userID);
        Task<Exam> Update(Exam request);
        Task<bool> Delete(int examID);
        Task<int> IncreaseAttemps(int examID);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest request);
        Task<List<Exam>> GetAll();
        Task<Exam> GetByID(int id);
    }
}
