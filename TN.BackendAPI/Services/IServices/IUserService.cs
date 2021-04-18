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
        Task<ResponseBase<List<AppUser>>> GetAll();
        Task<ResponseBase<AppUser>> GetByID(int id);
        Task<ResponseBase<AppUser>> EditProfile(int id, UserViewModel user);
        Task<ResponseBase<AppUser>> EditUserInfo(int id, UserViewModel user);
        Task<ResponseBase<bool>> DeleteUser(int id);
        Task<ResponseBase<bool>> RestoreUser(int id);
        Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model);
    }
}
