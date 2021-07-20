using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IExamManage
    {
        Task<ResponseBase<List<Exam>>> GetAll();
        Task<ResponseBase<PagedResult<Exam>>> GetAllPaging(ExamPagingRequest model);
        Task<ResponseBase<Exam>> GetByID(int examId);
        Task<ResponseBase<List<Exam>>> GetByCategory(int categoryId);
        Task<ResponseBase> Update(ExamModel model);
        Task<ResponseBase> Delete(int examId);
        Task<ResponseBase> DeleteMany(DeleteManyModel<int> lstId);
        Task<ResponseBase> Create(ExamModel model);
        Task<ResponseBase<ExamAttemps>> IncreaseAttemps(int id);
        Task<ResponseBase<CountExamModel>> CountExam();
    }
}
