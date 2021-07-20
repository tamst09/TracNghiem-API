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
        Task<ResponseBase> Create(Category model);
        Task<ResponseBase> Delete(int id);
        Task<ResponseBase> DeleteRange(DeleteManyModel<int> lstId);
        Task<ResponseBase> Update(Category model);
        Task<ResponseBase<CountCategoryModel>> Count();
    }
}
