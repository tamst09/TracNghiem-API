using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly TNDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public UserService(TNDbContext context, UserManager<AppUser> userManager)
        {
            _dbContext = context;
            _userManager = userManager;
        }

        public async Task<List<AppUser>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<NumberUserInfo> CountUser()
        {
            var result = new NumberUserInfo();
            result.TotalUser = await _dbContext.Users.CountAsync();
            result.TotalActiveUser = await _dbContext.Users.Where(u => u.isActive == true).CountAsync();
            result.TotalInactiveUser = result.TotalUser - result.TotalActiveUser;
            return result;
        } 
        public async Task<UserViewModel> GetByID(int id)
        {
            var user = _dbContext.Users.Where(u => u.Id == id && u.isActive == true)
                .Select(u => new UserViewModel() {
                Id = u.Id,
                UserName = u.UserName,
                AvatarURL = u.Avatar,
                AvatarPhoto = null,
                DoB = u.DoB,
                Email = u.Email,
                Name = u.Name,
                PhoneNumber = u.PhoneNumber
            }).FirstOrDefault();
            return user;
        }
        public async Task<PagedResult<UserViewModel>> GetListUserPaged(UserPagingRequest model)
        {
            // Query tat ca user hien co
            var allUser = await _dbContext.Users.ToListAsync();
            // check keyword de xem co dang tim kiem hay phan loai ko
            // sau do gan vao Query o tren
            if (!string.IsNullOrEmpty(model.keyword))
            {
                allUser = allUser.Where(u => u.UserName.Contains(model.keyword) ||
                u.Email.Contains(model.keyword) ||
                u.PhoneNumber.Contains(model.keyword) ||
                u.Name.Contains(model.keyword)
                ).ToList();
            }
            // get total row from query
            int totalrecord = allUser.Count;
            // get so trang
            int soTrang = (totalrecord % model.PageSize == 0) ? (totalrecord / model.PageSize) : (totalrecord / model.PageSize + 1);
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
                    AvatarURL = u.Avatar
                })
                .ToList();
            // return
            return new PagedResult<UserViewModel>() { Items = data, TotalRecords = totalrecord, TotalPages = soTrang, PageIndex = model.PageIndex, PageSize = model.PageSize };
        }
        public async Task<AppUser> EditUserInfo(UserViewModel model)
        {
            var user = await _dbContext.Users.FindAsync(model.Id);
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
            if (model.AvatarURL != null)
            {
                user.Avatar = model.AvatarURL;
            }
            try
            {
                await _dbContext.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateConcurrencyException)
            {
                return null;
            }
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return false;
            }

            user.isActive = false;
            user.RefreshToken = null;
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RestoreUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            user.isActive = true;
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<bool> AddUser(RegisterModel newUser)
        {
            if (string.IsNullOrEmpty(newUser.AvatarURL))
            {
                newUser.AvatarURL = "/images/cover/user/default_avatar.png";
            }
            var user = new AppUser()
            {
                UserName = newUser.UserName,
                Name = newUser.Name,
                Email = newUser.Email,
                DoB = newUser.DoB,
                PhoneNumber = newUser.PhoneNumber,
                Avatar = newUser.AvatarURL
            };
            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                RefreshToken refresh_token = GenerateRefreshToken();
                user.RefreshToken = refresh_token;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }
        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Token = Convert.ToBase64String(randomNumber);
            }
            //thoi han token
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
            return refreshToken;
        }
    }
}
