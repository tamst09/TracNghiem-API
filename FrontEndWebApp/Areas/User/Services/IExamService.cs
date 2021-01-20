using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IExamService
    {
        Task<ResponseBase<List<Exam>>> GetAll(string accessToken, string userID);
        Task<ResponseBase<PagedResult<Exam>>> GetAllExams(ExamPagingRequest model, string accessToken, string userID);
        Task<ResponseBase<List<Exam>>> GetOwnedExams(string accessToken, string userID);
        Task<ResponseBase<Exam>> Create(ExamModel model, int userID, string accessToken);
        Task<ResponseBase<int>> IncreaseAttemps(int id, string accessToken);
        Task<ResponseBase<Exam>> Update(int id, ExamModel model, string accessToken, string userID);
        Task<ResponseBase<Exam>> Delete(int id, string accessToken, string userID);
        Task<ResponseBase<Exam>> GetByID(int id, string accessToken, string userID);
    }
}
