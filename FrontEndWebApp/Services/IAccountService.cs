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
        Task<ResponseBase<UserViewModel>> UpdateProfile(int uid, UserViewModel model, string access_token);
        Task<ResponseBase<JwtResponse>> LoginFacebook(string accesstoken);
        Task<ResponseBase<JwtResponse>> LoginGoogle(string accesstoken);
        ClaimsPrincipal ValidateToken(string token);
        Task<ResponseBase<UserViewModel>> GetUserInfo(int userId, string access_token);
        Task<ResponseBase<UserViewModel>> AddPassword(ResetPasswordModel model);
        Task<ResponseBase<string>> GetResetPasswordCode(ForgotPasswordModel model);
        Task<ResponseBase<string>> ResetPassword(string resetCode, ResetPasswordModel model);
        Task<ResponseBase<string>> ChangePassword(string userID, ChangePasswordModel model, string accessToken);
    }
}
