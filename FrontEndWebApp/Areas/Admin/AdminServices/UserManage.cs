using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.AdminServices
{
    public class UserManage : IUserManage
    {
        public Task<List<UserViewModel>> GetListUser(string access_token)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model, string access_token)
        {
            throw new NotImplementedException();
        }
    }
}
