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
        Task<JwtResponse> Register(RegisterModel model);
        Task<List<AppUser>> GetAll();
        Task<AppUser> GetByID(int id);
        Task<JwtResponse> Login(LoginModel model);
        Task<AppUser> EditUserInfo(int id, UserViewModel user);
        Task<bool> DeleteUser(int id);
        Task<bool> RestoreUser(int id);
        Task<string> ResetPassword(ForgotPasswordModel model);
        Task<bool> ResetPasswordConfirm(ResetPasswordModel model);
        Task<AppUser> GetUserByAccessToken(string accessToken);
        Task<AppUser> AddPassword(ResetPasswordModel model);
        RefreshToken GenerateRefreshToken();
        Task<RefreshToken> GetRefreshTokenByAccessToken(string accessToken);
        bool ValidateRefreshToken(AppUser user, string refreshToken);
        Task<string> GenerateAccessTokenWithRefressToken(RefreshAccessTokenRequest refreshRequest);
        Task<JwtResponse> LoginWithFacebookToken(string accessToken);
        Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model);
    }
}
