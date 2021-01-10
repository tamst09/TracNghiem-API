using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IExamManage
    {
        Task<List<Exam>> GetAll(string accessToken);
        Task<PagedResult<Exam>> GetAllPaging(ExamPagingRequest model, string accessToken);
        Task<Exam> GetByID(int id, string accessToken);
        Task<bool> Update(Exam request, string accessToken);
        Task<bool> Delete(int examID, string accessToken);

        Task<Exam> Create(Exam request, int userID, string accessToken);
        Task<int> IncreaseAttemps(int examID, string accessToken);
    }
}
