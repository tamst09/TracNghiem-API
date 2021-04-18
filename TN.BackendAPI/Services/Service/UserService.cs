using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Services.Service
{
    public class UserService : IUserService
    {
        private readonly TNDbContext _db;
        public UserService(TNDbContext db)
        {
            _db = db;
        }

        //=============================== MANAGE USER ==========================================
        public async Task<ResponseBase<List<AppUser>>> GetAll()
        {
            var allUsers = await _db.Users.ToListAsync();
            return new ResponseBase<List<AppUser>>() {
                data = allUsers,
                success = true
            }; 
        }
        public async Task<ResponseBase<AppUser>> GetByID(int id)
        {
            var user = await _db.Users.FindAsync(id);
            return new ResponseBase<AppUser>()
            {
                data = user,
                success = true
            };
        }
        public async Task<ResponseBase<PagedResult<UserViewModel>>> GetListUserPaged(UserPagingRequest model)
        {
            // Query tat ca user hien co
            var allUser = await _db.Users.ToListAsync();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allUser = allUser.Where(
                    u => u.UserName.Contains(model.keyword) ||
                    u.Email.Contains(model.keyword) ||
                    u.PhoneNumber.Contains(model.keyword) ||
                    u.Name.Contains(model.keyword)
                ).ToList();
            }
            // get total row from query
            int totalRecords = allUser.Count;
            // get so trang
            int totalPages = (totalRecords % model.PageSize == 0) ? (totalRecords / model.PageSize) : (totalRecords / model.PageSize + 1);
            // get data and paging
            var data = allUser.Skip((model.PageIndex - 1) * model.PageSize)
                .Take(model.PageSize)
                .Select(u => new UserViewModel()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    DoB = u.DoB,
                    PhoneNumber = u.PhoneNumber,
                    UserName = u.UserName,
                    isActive = u.isActive,
                    Avatar = u.Avatar
                })
                .ToList();
            // return
            return new ResponseBase<PagedResult<UserViewModel>>()
            {
                success = true,
                data = new PagedResult<UserViewModel>()
                {
                    Items = data,
                    TotalRecords = totalRecords,
                    TotalPages = totalPages,
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                }
            };
        }
        public async Task<ResponseBase<AppUser>> EditProfile(int id, UserViewModel model)
        {
            if (id != model.Id)
            {
                return new ResponseBase<AppUser>()
                {
                    success = false,
                    msg = "Invalid user ID"
                };
            }
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return new ResponseBase<AppUser>()
                {
                    success = false,
                    msg = "Not found"
                };
            }
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
            }
            if (model.DoB != null)
            {
                user.DoB = model.DoB;
            }
            if (model.Email != null)
            {
                user.Email = model.Email;
            }
            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (model.Avatar != null)
            {
                user.Avatar = model.Avatar;
            }
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<AppUser>()
                {
                    data = user,
                    success = true,
                    msg = "Updated successfully"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<AppUser>()
                {
                    success = false,
                    msg = e.Message
                };
            }
            
        }
        public async Task<ResponseBase<AppUser>> EditUserInfo(int id, UserViewModel model)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return new ResponseBase<AppUser>()
                {
                    success = false,
                    msg = "Not found"
                };
            }
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Name))
            {
                user.Name = model.Name;
            }
            if (model.DoB != null)
            {
                user.DoB = model.DoB;
            }
            if (model.Email != null)
            {
                user.Email = model.Email;
            }
            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
            }
            if (model.Avatar != null)
            {
                user.Avatar = model.Avatar;
            }
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<AppUser>()
                {
                    data = user,
                    success = true,
                    msg = "Updated successfully"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<AppUser>()
                {
                    success = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<bool>> DeleteUser(int id)
        {
            var user = await _db.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return new ResponseBase<bool>() 
                {
                    success = false,
                    data = false,
                    msg = "Not found"
                };
            }
            user.isActive = false;
            user.RefreshToken = null;
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<bool>()
                {
                    success = true,
                    data = true,
                    msg = "Deleted successfully"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = e.Message
                };
            }
        }
        public async Task<ResponseBase<bool>> RestoreUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = "Not found"
                };
            }
            user.isActive = true;
            try
            {
                await _db.SaveChangesAsync();
                return new ResponseBase<bool>()
                {
                    success = true,
                    data = true,
                    msg = "Restored"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = e.Message
                };
            }
        }
        //--------------------------------------------------------------------------------------
    }
}
