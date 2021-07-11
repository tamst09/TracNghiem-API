using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IFavoriteExamService
    {
        Task<ResponseBase<List<Exam>>> GetExams(int userId, string accessToken);
        Task<bool> Add(int userId, int examId, string accessToken);
        Task<bool> Delete(int userId, int examId, string accessToken);
    }
}
