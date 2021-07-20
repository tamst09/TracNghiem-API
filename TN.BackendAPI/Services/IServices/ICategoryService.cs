using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task<bool> Create(Category request);
        Task<bool> Update(Category request);
        Task<bool> Delete(int id);
        Task<bool> DeleteMany(DeleteManyModel<int> lstCategoryId);
        Task<List<Category>> GetAll();
        Task<Category> GetByID(int id);
        Task<CountCategoryModel> CountCategory();
    }
}
