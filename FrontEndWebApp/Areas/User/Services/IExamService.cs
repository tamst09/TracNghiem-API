using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.User.Services
{
    public interface IExamService
    {
        Task<ResponseBase<List<Exam>>> GetAll();
        Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model);
        Task<ResponseBase<List<Exam>>> GetOwned();
        Task<ResponseBase> Create(ExamModel model);
        Task<ResponseBase<ExamAttemps>> IncreaseAttemps(int id);
        Task<ResponseBase> Update(ExamModel model);
        Task<ResponseBase> Delete(int id);
        Task<ResponseBase<Exam>> GetByID(int id);
    }
}
