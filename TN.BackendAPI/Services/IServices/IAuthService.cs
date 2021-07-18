using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseBase<JwtResponse>> Register(RegisterModel model);
        Task<ResponseBase<JwtResponse>> Login(LoginModel model);
        Task<ResponseBase<UserInfo>> GetProfile(int userId);
        Task<AppUser> UpdateProfile(UserViewModel user);
        Task<string> ResetPassword(ForgotPasswordModel model);
        Task<bool> ResetPasswordConfirm(ResetPasswordModel model);
        Task<AppUser> GetUserByAccessToken(string accessToken);
        Task<AppUser> AddPassword(ResetPasswordModel model);
        Task<string> ChangePassword(int userID, ChangePasswordModel model);
        Task<RefreshToken> GetRefreshTokenByAccessToken(string accessToken);
        bool ValidateRefreshToken(AppUser user, string refreshToken);
        Task<string> GenerateAccessTokenWithRefressToken(RefreshAccessTokenRequest refreshRequest);
        Task<JwtResponse> LoginWithFacebookToken(string accessToken);
        Task<JwtResponse> LoginWithGoogleToken(string token, string email, string name, string avatar, string ggID);
    }
}
