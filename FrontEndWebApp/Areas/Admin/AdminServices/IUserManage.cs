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
        Task<List<UserViewModel>> GetListUser(string access_token); // lay toan bo user
        Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model, string access_token); // lay user theo phan trang
    }
}
