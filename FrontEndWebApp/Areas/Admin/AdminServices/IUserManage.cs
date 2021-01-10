using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public interface IUserManage
    {
        Task<NumberUserInfo> GetNumberOfUsers(string access_token); // lay toan bo user
        Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model, string accessToken); // lay user theo phan trang
        Task<UserViewModel> GetOneUser(string access_token, int id);
        Task<JwtResponse> CreateUser(UserViewModel model, string accessToken);
        Task<bool> LockUser(int id, string accessToken);
        Task<bool> RestoreUser(int id, string accessToken);
    }
}
