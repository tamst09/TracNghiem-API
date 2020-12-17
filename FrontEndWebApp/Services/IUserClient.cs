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
        Task<AppUser> Register(RegisterModel model);
        Task<AppUser> UpdateProfile(int uid, RegisterModel model);
        Task<CreateFacebookUserResult> LoginFacebook(string accesstoken);
        ClaimsPrincipal ValidateToken(string token);
        Task<AppUser> GetUserInfo(int userId);
    }
}
