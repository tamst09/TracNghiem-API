using System.Net.Http;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly IApiHelper _apiHelper;

        public AccountService(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<ResponseBase<JwtResponse>> Authenticate(LoginModel model)
        {
            var res = await _apiHelper.QueryAsync<LoginModel, JwtResponse>(HttpMethod.Post, "/api/users/login", model);
            return res;
        }

        public async Task<ResponseBase<UserViewModel>> GetUserInfo(int userId)
        {
            var res = await _apiHelper.NonBodyQueryAsync<UserViewModel>(HttpMethod.Get, $"/api/users/{userId}");
            return res;
        }

        public async Task<ResponseBase<JwtResponse>> LoginFacebook(string accesstoken)
        {
            var res = await _apiHelper.QueryAsync<string, JwtResponse>(HttpMethod.Post, "/api/users/loginfb", accesstoken);
            return res;
        }

        public async Task<ResponseBase<JwtResponse>> LoginGoogle(string accesstoken)
        {
            var res = await _apiHelper.QueryAsync<string, JwtResponse>(HttpMethod.Post, "/api/users/logingg", accesstoken);
            return res;
        }

        public async Task<ResponseBase<JwtResponse>> Register(RegisterModel model)
        {
            var res = await _apiHelper.QueryAsync<RegisterModel, JwtResponse>(HttpMethod.Post, "/api/users/register", model);
            return res;
        }

        public async Task<ResponseBase> UpdateProfile(UserViewModel model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Put, "/api/users", model);
            return res;
        }

        public async Task<ResponseBase> AddPassword(ResetPasswordModel model)
        {
            var res = await _apiHelper.CommandAsync(HttpMethod.Put, "/api/Users/AddPassword", model);
            return res;
        }

        public async Task<ResponseBase<string>> GetResetPasswordCode(ForgotPasswordModel model)
        {
            var res = await _apiHelper.QueryAsync<ForgotPasswordModel, string>(HttpMethod.Post, "/api/Users/GetResetCode", model);
            return res;
        }
        public async Task<ResponseBase<string>> ResetPassword(ResetPasswordModel model)
        {
            var res = await _apiHelper.QueryAsync<ResetPasswordModel, string>(HttpMethod.Post, "/api/Users/resetpass", model);
            return res;
        }

        public async Task<ResponseBase<string>> ChangePassword(int userID, ChangePasswordModel model)
        {
            var res = await _apiHelper.QueryAsync<ChangePasswordModel, string>(HttpMethod.Post, $"/api/Users/ChangePass?userId={userID}", model);
            return res;
        }

        public async Task<ResponseBase<UserInfo>> GetUserInfoByToken()
        {
            var res = await _apiHelper.NonBodyQueryAsync<UserInfo>(HttpMethod.Get, $"/api/Users/Profile");
            return res;
        }
    }
}
