using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Interface
{
    public interface IQuestionService
    {
        Task<PagedResultVM<Question>> GetAllQuestionPaging(GetQuestionPagingRequest request);
        Task<List<Question>> GetListQuestionByExam(int examID);
        Task<Question> Create(Question request, int examID);
        Task<Question> Update(Question request);
        Task<bool> Delete(int questionID);
        Task<Question> GetByID(int id);
    }
}
