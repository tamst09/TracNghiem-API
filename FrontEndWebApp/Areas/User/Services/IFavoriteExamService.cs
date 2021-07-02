using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IFavoriteExamService
    {
        Task<List<Exam>> GetExams(int userId);
        Task<bool> Add(int userId, int examId);
        Task<bool> Delete(int userId, int examId);
    }
}
