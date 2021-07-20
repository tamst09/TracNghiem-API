using FrontEndWebApp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class UserManage : IUserManage
    {
        private readonly IApiHelper _apiHelper;

        public UserManage(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase> CreateUser(UserViewModel model)
        {
            var res = await _apiHelper.CommandAsync<UserViewModel>(HttpMethod.Post, "/api/Users/CreateUser", model);
            return res;
        }


        public async Task<ResponseBase<NumberUserInfo>> CountUser()
        {
            var res = await _apiHelper.NonBodyQueryAsync<NumberUserInfo>(HttpMethod.Get, "/api/Users/Count");
            return res;
        }

        public async Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model)
        {
            var res = await _apiHelper.QueryAsync<UserPagingRequest, PagedResult<UserViewModel>>(HttpMethod.Post, "/api/Users/Paged", model);
            return res;
        }

        public async Task<ResponseBase<UserViewModel>> GetOneUser(int id)
        {
            var res = await _apiHelper.NonBodyQueryAsync<UserViewModel>(HttpMethod.Get, $"/api/Users/{id}");
            return res;
        }

        public async Task<ResponseBase> LockUser(int id)
        {
            var res = await _apiHelper.NonBodyCommandAsync(HttpMethod.Post, $"/api/LockUser/{id}");
            return res;
        }
        public async Task<ResponseBase> RestoreUser(int id)
        {
            var res = await _apiHelper.NonBodyCommandAsync(HttpMethod.Post, $"/api/RestoreUser/{id}");
            return res;
        }
    }
}
