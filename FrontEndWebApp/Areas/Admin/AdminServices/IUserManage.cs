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
        Task<ResponseBase<NumberUserInfo>> GetNumberOfUsers(string access_token); // lay toan bo user
        Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model, string accessToken); // lay user theo phan trang
        Task<ResponseBase<UserViewModel>> GetOneUser(string access_token, int id);
        Task<ResponseBase<JwtResponse>> CreateUser(UserViewModel model, string accessToken);
        Task<ResponseBase<string>> LockUser(int id, string accessToken);
        Task<ResponseBase<string>> RestoreUser(int id, string accessToken);
        Task<ResponseBase<UserViewModel>> UpdateUserInfo(int uid, UserViewModel model, string access_token);
    }
}
