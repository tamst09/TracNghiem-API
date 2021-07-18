using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.FacebookAuth;

namespace TN.BackendAPI.Services.IServices
{
    public interface IUserService
    {
        Task<ResponseBase<JwtResponse>> Register(RegisterModel model);
        Task<List<AppUser>> GetAll();
        Task<NumberUserInfo> CountUser();
        Task<AppUser> GetByID(int id);
        Task<ResponseBase<JwtResponse>> Login(LoginModel model);
        Task<AppUser> EditUserInfo(UserViewModel user);
        Task<bool> DeleteUser(int id);
        Task<bool> RestoreUser(int id);
        Task<string> ResetPassword(ForgotPasswordModel model);
        Task<bool> ResetPasswordConfirm(ResetPasswordModel model);
        Task<AppUser> GetUserByAccessToken(string accessToken);
        Task<AppUser> AddPassword(ResetPasswordModel model);
        Task<string> ChangePassword(int userID, ChangePasswordModel model);
        RefreshToken GenerateRefreshToken();
        Task<RefreshToken> GetRefreshTokenByAccessToken(string accessToken);
        bool ValidateRefreshToken(AppUser user, string refreshToken);
        Task<string> GenerateAccessTokenWithRefressToken(RefreshAccessTokenRequest refreshRequest);
        Task<JwtResponse> LoginWithFacebookToken(string accessToken);
        Task<JwtResponse> LoginWithGoogleToken(string token, string email, string name, string avatar, string ggID);
        Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model);
    }
}
