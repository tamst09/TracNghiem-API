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
        Task<List<AppUser>> GetAll();
        Task<NumberUserInfo> CountUser();
        Task<UserViewModel> GetByID(int id);
        Task<AppUser> EditUserInfo(UserViewModel user);
        Task<bool> AddUser(RegisterModel newUser);
        Task<bool> DeleteUser(int id);
        Task<bool> RestoreUser(int id);
        Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model);
    }
}
