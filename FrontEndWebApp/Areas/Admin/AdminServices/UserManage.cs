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
        private readonly IHttpClientFactory _httpClientFactory;
        private HttpClient _httpClient;

        public UserManage(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = _httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(ConstStrings.BASE_URL_API);
        }

        public async Task<ResponseBase<JwtResponse>> CreateUser(UserViewModel model, string access_token)
        {
            if (access_token != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/users/CreateUser/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                ResponseBase<JwtResponse> jwtResponse = JsonConvert.DeserializeObject<ResponseBase<JwtResponse>>(result);
                return jwtResponse;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<UserViewModel>> UpdateUserInfo(int uid, UserViewModel model, string access_token)
        {
            if (!string.IsNullOrEmpty(access_token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            model.Id = uid;
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Users/EditUser", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadAsStringAsync();
                ResponseBase<UserViewModel> uservm = JsonConvert.DeserializeObject<ResponseBase<UserViewModel>>(user);
                return uservm;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<NumberUserInfo>> GetNumberOfUsers(string access_token)
        {
            if (access_token != null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            var response = await _httpClient.GetAsync("/api/Users/GetNumber");
            if (response.IsSuccessStatusCode)
            {
                var resContent = await response.Content.ReadAsStringAsync();
                ResponseBase<NumberUserInfo> numberUserInfo = JsonConvert.DeserializeObject<ResponseBase<NumberUserInfo>>(resContent);
                return numberUserInfo;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model, string access_token)
        {
            if (access_token != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var json = JsonConvert.SerializeObject(model);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/users/paged/", httpContent);
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                ResponseBase<PagedResult<UserViewModel>> pagedResult = JsonConvert.DeserializeObject<ResponseBase<PagedResult<UserViewModel>>>(body);
                return pagedResult;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<UserViewModel>> GetOneUser(string access_token, int id)
        {
            if (access_token != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            var response = await _httpClient.GetAsync("/api/users/"+id.ToString());
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseBase<UserViewModel> user = JsonConvert.DeserializeObject<ResponseBase<UserViewModel>>(responseContent);
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<ResponseBase<string>> LockUser(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync("/api/users/LockUser/" + id.ToString(), null);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseBase<string> deleteResult = JsonConvert.DeserializeObject<ResponseBase<string>>(responseContent);
                return deleteResult;
            }
            else
            {
                return null;
            }
        }
        public async Task<ResponseBase<string>> RestoreUser(int id, string accessToken)
        {
            if (accessToken != null)
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.PostAsync("/api/users/RestoreUser/" + id.ToString(), null);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                ResponseBase<string> deleteResult = JsonConvert.DeserializeObject<ResponseBase<string>>(responseContent);
                return deleteResult;
            }
            else
            {
                return null;
            }
        }
    }
}
