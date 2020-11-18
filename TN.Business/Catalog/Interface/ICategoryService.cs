using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace TN.Business.Catalog.Interface
{
    public interface ICategoryService
    {
        Task<int> Create(Category request);
        Task<int> Update(int id, Category request);
        Task<int> Delete(int examID);
        Task<List<Category>> GetAll();
        Task<Category> Get(int id);
    }
}
