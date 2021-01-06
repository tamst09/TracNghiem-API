using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace TN.BackendAPI.Services.IServices
{
    public interface ICategoryService
    {
        Task<Category> Create(Category request);
        Task<Category> Update(Category request);
        Task<bool> Delete(int id);
        Task<List<Category>> GetAll();
        Task<Category> GetByID(int id);
    }
}
