using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Interface
{
    public interface IExamService
    {
        Task<Exam> create(Exam request, int userID);
        Task<Exam> update(Exam request);
        Task<bool> delete(int examID);
        Task<int> increaseAttemps(int examID);
        Task<PagedResultVM<Exam>> getAllPaging(GetExamPagingRequest request);
        Task<List<Exam>> getAll();
        Task<Exam> getByID(int id);
    }
}
