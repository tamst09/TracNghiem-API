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

        // send code to email
        Task<ResponseBase<string>> SendResetPasswordCode(ForgotPasswordModel model);
        // use code to reset password
        Task<ResponseBase<bool>> ResetPassword(ResetPasswordModel model);

        //Task<ResponseBase<AppUser>> GetUserByAccessToken(string accessToken);
        Task<ResponseBase<bool>> AddPassword(AddPasswordModel model);
        Task<ResponseBase<bool>> ChangePassword(int userID, ChangePasswordModel model);
        //RefreshToken GenerateRefreshToken();
        Task<ResponseBase<RefreshToken>> GetRefreshTokenByAccessToken(string accessToken);
        //bool ValidateRefreshToken(AppUser user, string refreshToken);
        Task<ResponseBase<string>> GenerateAccessTokenWithRefreshToken(RefreshAccessTokenRequest refreshRequest);
        Task<ResponseBase<JwtResponse>> LoginWithFacebookToken(string accessToken);
        Task<ResponseBase<JwtResponse>> LoginWithGoogleToken(string token);
    }
}
