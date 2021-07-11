using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace TN.BackendAPI.Services.IServices
{
    public interface IFavoriteExamService
    {
        Task<List<Exam>> GetByUser(int userId);
        Task<bool> Add(int userId, int examId);
        Task<bool> Delete(int userId, int examId);
    }
}
