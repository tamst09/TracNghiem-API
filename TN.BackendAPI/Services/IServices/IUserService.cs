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
        Task<JwtResponse> Register(RegisterModel request);
        Task<IEnumerable<AppUser>> GetAll();
        Task<AppUser> GetByID(int id);
        Task<JwtResponse> Authenticate(LoginModel request);
        Task<AppUser> EditUserInfo(int id, AppUser user);
        Task<bool> DeleteUser(int id);
        Task<string> ResetPassword(ForgotPasswordModel model);
        Task<string> ResetPasswordConfirm(ResetPasswordModel model);
        Task<AppUser> GetUserByAccessToken(string accessToken);
        RefreshToken GenerateRefreshToken();
        bool ValidateRefreshToken(AppUser user, string refreshToken);
        Task<string> GetNewAccessToken(RefreshAccessTokenRequest refreshRequest);
        Task<CreateFacebookUserResult> GetUserWithFacebookToken(string accessToken);
    }
}
