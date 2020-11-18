using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Interface
{
    public interface IManageExamService
    {
        Task<int> Create(Exam request, int user);
        Task<int> Update(Exam request);
        Task<int> Delete(int examID);
        Task IncreaseAttemps(int examID);
        Task<PagedResultVM<Exam>> GetAllPaging(GetExamPagingRequest request);
        Task<List<Exam>> GetAll();
        Task<Exam> GetByID(int id);
    }
}
