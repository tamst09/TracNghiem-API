using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface ICategoryManage
    {
        Task<ResponseBase<List<Category>>> GetAll();
        Task<ResponseBase<Category>> GetByID(int id);
        Task<ResponseBase<Category>> Create(Category model, string accessToken);
        Task<ResponseBase<string>> Delete(int id, string accessToken);
        Task<ResponseBase<string>> DeleteRange(DeleteRangeModel<int> lstId, string accessToken);
        Task<ResponseBase<Category>> Update(int id, Category model, string accessToken);
        Task<ResponseBase<List<Exam>>> GetAllExams(int id, string accessToken);
    }
}
