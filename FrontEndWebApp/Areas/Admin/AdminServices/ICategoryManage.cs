using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface ICategoryManage
    {
        Task<List<Category>> GetAll();
        Task<Category> GetByID(int id);
        Task<bool> Create(Category model, string accessToken);
        Task<bool> Delete(int id, string accessToken);
        Task<Category> Update(int id, Category model, string accessToken);
    }
}
