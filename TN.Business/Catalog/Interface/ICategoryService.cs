using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace TN.Business.Catalog.Interface
{
    public interface ICategoryService
    {
        Task<Category> create(Category request);
        Task<Category> update(Category request);
        Task<bool> delete(int examID);
        Task<List<Category>> getAll();
        Task<Category> getByID(int id);
    }
}
