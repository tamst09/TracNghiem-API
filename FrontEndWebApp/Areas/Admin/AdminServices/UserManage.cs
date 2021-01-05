using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class UserManage : IUserManage
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public UserManage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<JwtResponse> CreateUser(UserViewModel model, string access_token)
        {
            if (access_token != null)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/users/CreateUser/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                JwtResponse jwtResponse = JsonConvert.DeserializeObject<JwtResponse>(result);
                return jwtResponse;
            }
            else
            {
                return null;
            }
        }

        public Task<List<UserViewModel>> GetListUser(string access_token)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model, string access_token)
        {
            if (access_token != null)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + access_token);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/users/paged", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var lstuser = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonConvert.DeserializeObject<PagedResult<UserViewModel>>(lstuser);
                return pagedResult;
            }
            else
            {
                var result = new PagedResult<UserViewModel>();
                result.Items = null;
                result.PageIndex = 1;
                result.PageSize = 10;
                result.TotalPages = 1;
                result.TotalRecords = 0;
                return result;
            }
        }

        public async Task<bool> LockUser(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var response = await _httpClient.PostAsync("/api/users/LockUser/" + id.ToString(), null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> RestoreUser(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            var response = await _httpClient.PostAsync("/api/users/RestoreUser/" + id.ToString(), null);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
