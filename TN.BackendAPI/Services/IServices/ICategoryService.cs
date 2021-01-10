using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;

namespace TN.BackendAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category> Create(Category request);
        Task<Category> Update(Category request);
        Task<bool> Delete(int id);
        Task<bool> DeleteListCategory(DeleteRangeModel<int> lstCategoryId);
        Task<List<Category>> GetAll();
        Task<Category> GetByID(int id);
    }
}
