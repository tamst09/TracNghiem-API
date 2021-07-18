using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.FacebookAuth;

namespace FrontEndWebApp.Services
{
    public interface IAccountService
    {
        Task<ResponseBase<JwtResponse>> Authenticate(LoginModel model);
        Task<ResponseBase<JwtResponse>> Register(RegisterModel model);
        Task<ResponseBase> UpdateProfile(UserViewModel model);
        Task<ResponseBase<JwtResponse>> LoginFacebook(string accesstoken);
        Task<ResponseBase<JwtResponse>> LoginGoogle(string accesstoken);
        Task<ResponseBase<UserViewModel>> GetUserInfo(int userId);
        Task<ResponseBase> AddPassword(ResetPasswordModel model);
        Task<ResponseBase<string>> GetResetPasswordCode(ForgotPasswordModel model);
        Task<ResponseBase<string>> ResetPassword(ResetPasswordModel model);
        Task<ResponseBase<string>> ChangePassword(int userID, ChangePasswordModel model);
        Task<ResponseBase<UserInfo>> GetUserInfoByToken();
    }
}
