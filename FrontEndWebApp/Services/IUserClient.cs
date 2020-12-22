using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.FacebookAuth;

namespace FrontEndWebApp.Services
{
    public interface IUserClient
    {
        Task<string> Authenticate(LoginModel model);
        Task<JwtResponse> Register(RegisterModel model);
        Task<UserViewModel> UpdateProfile(int uid, UserViewModel model);
        Task<JwtResponse> LoginFacebook(string accesstoken);
        ClaimsPrincipal ValidateToken(string token);
        Task<UserViewModel> GetUserInfo(int userId);
        Task<UserViewModel> AddPassword(ResetPasswordModel model);
    }
}
