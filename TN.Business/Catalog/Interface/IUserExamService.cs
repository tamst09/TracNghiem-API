using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Interface
{
    public interface IUserExamService
    {
        Task<int> Create(Exam request, int userID);
        Task<int> Update(Exam request, int userID);
        Task<int> Delete(int examID, int userID);
        Task IncreaseAttemps(int examID);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest request);
        Task<List<Exam>> GetAll();
        Task<Exam> GetByID(int id);
    }
}
