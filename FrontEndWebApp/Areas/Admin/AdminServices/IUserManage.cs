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
        Task<ResponseBase<NumberUserInfo>> CountUser();
        Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model);
        Task<ResponseBase<UserViewModel>> GetOneUser(int id);
        Task<ResponseBase> CreateUser(UserViewModel model);
        Task<ResponseBase> LockUser(int id);
        Task<ResponseBase> RestoreUser(int id);
    }
}
