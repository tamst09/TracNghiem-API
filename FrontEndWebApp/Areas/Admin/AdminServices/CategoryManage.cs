using FrontEndWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class CategoryManage : ICategoryManage
    {
        private readonly IApiHelper _apiHelper;

        public CategoryManage(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase<CountCategoryModel>> Count()
        {
            var res = await _apiHelper.NonBodyQueryAsync<CountCategoryModel>(HttpMethod.Get, "/api/Categories/Count");
            return res;
        }

        public async Task<ResponseBase> Create(Category model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Categories", model);
            return res;
        }

        public async Task<ResponseBase> Delete(int id)
        {
            var res = await _apiHelper.NonBodyCommandAsync(HttpMethod.Delete, $"/api/Categories/{id}");
            return res;
        }

        public async Task<ResponseBase> DeleteRange(DeleteManyModel<int> lstId)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Post, "/api/Categories/DeleteRange", lstId);
            return res;
        }

        public async Task<ResponseBase<List<Category>>> GetAll()
        {
            var res = await _apiHelper.NonBodyQueryAsync<List<Category>>(HttpMethod.Get, "/api/Categories");
            return res;
        }

        public async Task<ResponseBase<Category>> GetByID(int id)
        {
            var res = await _apiHelper.NonBodyQueryAsync<Category>(HttpMethod.Get, $"/api/Categories/{id}");
            return res;
        }

        public async Task<ResponseBase> Update(Category model)
        {
            var res = await _apiHelper.CommandAsync<Category>(HttpMethod.Put, $"/api/Categories", model);
            return res;
        }

    }
}
