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
        Task<List<Question>> getListQuestionByExam(int examID);
        Task<Question> create(Question request, int examID);
        Task<Question> update(Question request);
        Task<bool> delete(int questionID);
        Task<Question> getByID(int id);
    }
}
