using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IExamService
    {
        Task<ResponseBase<PagedResult<Exam>>> GetAllExams(ExamPagingRequest model, string accessToken, string userID);
    }
}
