using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.Data.Model;
using TN.Data.ViewModel;
using TN.ViewModels.Catalog.Users;

namespace TN.Business.Catalog.Interface
{
    public interface IUserService
    {
        Task<JwtResponse> register(RegisterRequest request);
        Task<IEnumerable<AppUser>> getAll();
        Task<AppUser> getByID(int id);
        Task<string> authenticate(LoginRequest request);
        Task<AppUser> editUserInfo(int id, AppUser user);
        Task<AppUser> createUser(AppUser user);
        Task<bool> deleteUser(int id);
        Task<string> forgotPassword(ForgotPasswordModel model);
        Task<string> resetPassword(ResetPasswordModel model);
    }
}
