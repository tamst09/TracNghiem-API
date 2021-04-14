using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task<ResponseBase<Category>> Create(Category request);
        Task<ResponseBase<Category>> Update(Category request);
        Task<ResponseBase<bool>> Delete(int categoryID);
        Task<ResponseBase<bool>> DeleteMany(DeleteRangeModel<int> lstCategoryId);
        Task<ResponseBase<List<Category>>> GetAll();
        Task<ResponseBase<Category>> GetOne(int id);
    }
}
