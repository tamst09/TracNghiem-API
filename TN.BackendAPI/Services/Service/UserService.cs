using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using TN.ViewModels.Settings;

namespace TN.BackendAPI.Services.Service
{
    public class UserService : IUserService
    {
        private readonly TNDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IFacebookAuth _facebookAuth;
        private readonly IEmailSender _emailSender;
        public UserService(
            TNDbContext db,
            UserManager<AppUser> userManager,
            IConfiguration config, IFacebookAuth facebookAuth, IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _config = config;
            _facebookAuth = facebookAuth;
            _emailSender = emailSender;
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


        //=============================== AUTHENTICATION =======================================
        public async Task<ResponseBase<JwtResponse>> Login(LoginModel model)
        {
            var userLogin = await _db.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (userLogin == null) return new ResponseBase<JwtResponse>() { success = false, msg = "Username is invalid" };

            if (userLogin.isActive == false) return new ResponseBase<JwtResponse>() { success = false, msg = "This account was locked" };

            var IsValidPassword = await _userManager.CheckPasswordAsync(userLogin, model.Password);
            if (!IsValidPassword)
            {
                return new ResponseBase<JwtResponse>() { success = false, msg = "Wrong password" }; ;
            }
            // tao access_token
            string access_token = GenerateAccessToken(userLogin);
            // cap nhat refreshToken
            if (userLogin.RefreshTokenValue != null)
            {
                userLogin.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                await _db.SaveChangesAsync();
                return new ResponseBase<JwtResponse>()
                { 
                    data = new JwtResponse()
                    {
                        Access_Token = access_token,
                        Refresh_Token = userLogin.RefreshToken.Token
                    },
                    success = true,
                    msg = "Login successfully"
                };
            }
            // dang nhap lan dau, chua co refreshToken
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                userLogin.RefreshToken = refreshToken;
                _db.RefreshTokens.Add(refreshToken);
                await _db.SaveChangesAsync();
                return new ResponseBase<JwtResponse>()
                {
                    data = new JwtResponse()
                    {
                        Access_Token = access_token,
                        Refresh_Token = userLogin.RefreshToken.Token
                    },
                    success = true,
                    msg = "Login successfully"
                };
            }
        }
        public async Task<ResponseBase<JwtResponse>> LoginWithFacebookToken(string fbAccessToken)
        {
            // xac thuc token cua fb
            var validatedToken = await _facebookAuth.ValidateAccessTokenAsync(fbAccessToken);
            if (!validatedToken.Data.IsValid)
            {
                return new ResponseBase<JwtResponse>()
                {
                    success = false,
                    msg = "Invalid token"
                };
            }

            // chua co tk
            bool isNewUser = true;
            // lay thong tin de tao tai khoan
            var FbUserInfo = await _facebookAuth.GetUserInfoAsync(fbAccessToken);
            // truong hop ko co mail thi tao chuoi mail fake
            if(string.IsNullOrEmpty(FbUserInfo.Email))
            {
                FbUserInfo.Email = FbUserInfo.FirstName.ToLower() + "_" + FbUserInfo.LastName.ToLower();
            }
            // tim kiem user theo userName va Email
            var user = await _db.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.UserName==FbUserInfo.Email || u.Email == FbUserInfo.Email);
            // khong tim thay thi tim theo social ID
            if (user == null)
            {
                var findFacebookUID = await _db.UserTokens.FirstOrDefaultAsync(p => p.LoginProvider == "Facebook" && p.Name == "fbID" && p.Value == FbUserInfo.Id);

                if (findFacebookUID != null)
                {
                    user = await _userManager.FindByIdAsync(findFacebookUID.UserId.ToString());
                }
            }

            // chua co tai khoan
            if (user == null)
            {
                isNewUser = true;
                user = new AppUser()
                {
                    Name = FbUserInfo.FirstName +" "+ FbUserInfo.LastName,
                    Email = FbUserInfo.Email,
                    UserName = FbUserInfo.Email,
                    isActive = true,
                    Avatar = "/images/cover/user/default_avatar.png"
                };
                var createdResult = await _userManager.CreateAsync(user);
                if (createdResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Facebook", user.Email, "Facebook"));
                    await _userManager.AddToRoleAsync(user, "user");
                    await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "fbID", FbUserInfo.Id);
                }
                else
                {
                    return null;
                }
            }
            // da co tai khoan
            else
            {
                isNewUser = false;
                var checkProdiverResult = await _userManager.FindByLoginAsync("Facebook", FbUserInfo.Email);
                // ko phai tk facebook
                if (checkProdiverResult == null)
                {
                    var addloginTask = await _userManager.AddLoginAsync(user, new UserLoginInfo("Facebook", user.Email, "Facebook"));

                    var checkAuthTokenTask = await _userManager.GetAuthenticationTokenAsync(user, "Facebook", "fbID");
                    if (checkAuthTokenTask == null)
                    {
                        await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "fbID", FbUserInfo.Id);
                    }
                }
            }
            // generate new access_token
            string access_token = GenerateAccessToken(user);
            // access_token is available
            if (user.RefreshTokenValue != null)
            {
                user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                await _db.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = user.RefreshToken.Token, isNewLogin = isNewUser };
            }
            // new login info
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                _db.RefreshTokens.Add(refreshToken);
                await _db.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = refreshToken.Token, isNewLogin = isNewUser };
            }
        }
        public async Task<JwtResponse> LoginWithGoogleToken(string token, string email, string name, string avatar, string ggID)
        {
            bool isNewUser = true;
            var user = await _db.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                var userGetByGoogleID = await _db.UserTokens.FirstOrDefaultAsync(p => p.LoginProvider == "Google" && p.Name == "ggID" && p.Value == ggID);
                if (userGetByGoogleID != null)
                {
                    user = await _userManager.FindByIdAsync(userGetByGoogleID.UserId.ToString());
                }
            }

            // chua co tai khoan
            if (user == null)
            {
                user = new AppUser()
                {
                    Name = name,
                    Email = email,
                    UserName = email,
                    isActive = true,
                    Avatar = avatar
                };
                var createdResult = await _userManager.CreateAsync(user);
                if (createdResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", user.Email, "Google"));
                    await _userManager.AddToRoleAsync(user, "user");
                    await _userManager.SetAuthenticationTokenAsync(user, "Google", "ggID", ggID);
                }
                else
                {
                    return null;
                }
            }
            // da co tai khoan
            else
            {
                isNewUser = false;
                var checkProdiverResult = await _userManager.FindByLoginAsync("Google", email);
                // ko phai tk facebook
                if (checkProdiverResult == null)
                {
                    var addloginTask = await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", email, "Google"));

                    var checkAuthTokenTask = await _userManager.GetAuthenticationTokenAsync(user, "Google", "ggID");
                    if (checkAuthTokenTask == null)
                    {
                        await _userManager.SetAuthenticationTokenAsync(user, "Google", "ggID", ggID);
                    }
                }
            }
            // generate new access_token
            string access_token = GenerateAccessToken(user);
            // access_token is available
            if (user.RefreshTokenValue != null)
            {
                user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                await _db.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = user.RefreshToken.Token, isNewLogin = isNewUser };
            }
            // new login info
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                _db.RefreshTokens.Add(refreshToken);
                await _db.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = refreshToken.Token, isNewLogin = isNewUser };

            }
        }
        public async Task<JwtResponse> Register(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.AvatarPhotoURL))
            {
                model.AvatarPhotoURL = "/images/cover/user/default_avatar.png";
            }
            var user = new AppUser()
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                DoB = model.DoB,
                PhoneNumber = model.PhoneNumber,
                Avatar = model.AvatarPhotoURL
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                RefreshToken refresh_token = GenerateRefreshToken();
                user.RefreshToken = refresh_token;
                await _db.SaveChangesAsync();
                return new JwtResponse() { Access_Token = GenerateAccessToken(user), Refresh_Token = refresh_token.Token };
            }
            var error = result.Errors.First();
            return new JwtResponse() { Error = error.Description };
        }
        //--------------------------------------------------------------------------------------

        //=================================  TOKEN  ============================================
        // tao access token
        private string GenerateAccessToken(AppUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Tokens:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserID", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.Name),
                    new Claim(ClaimTypes.Role, roles)
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                Audience = _config["Tokens:Issuer"],
                Issuer = _config["Tokens:Issuer"]
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        // tao refresh token
        public RefreshToken GenerateRefreshToken()
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
        // Xac thuc han su dung refreshToken
        public bool ValidateRefreshToken(AppUser user, string refreshToken)
        {

            RefreshToken refreshTokenUser = _db.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefault();
            if (refreshTokenUser != null && refreshTokenUser.User == user && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }
        // lay ra user tu access_token gui len
        public async Task<AppUser> GetUserByAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Tokens:SecretKey"]);
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = true,
                    ValidAudience = _config["Tokens:Issuer"],
                    ValidIssuer = _config["Tokens:Issuer"]
                };
                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userName = principle.FindFirst(ClaimTypes.Name)?.Value;
                    return await _db.Users.Where(u => u.UserName == userName && u.isActive == true).Include(u => u.RefreshToken).FirstOrDefaultAsync();
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        //gia han access_token va tra ve client
        public async Task<string> GenerateAccessTokenWithRefressToken(RefreshAccessTokenRequest refreshRequest)
        {
            AppUser user = await GetUserByAccessToken(refreshRequest.AccessToken);
            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                var u = user.RefreshToken.ExpiryDate.Subtract(DateTime.UtcNow);
                if (u.TotalDays < 2)
                {
                    user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                }
                return GenerateAccessToken(user);
            }
            return null;
        }
        // lay refresh token bang accessToken
        public async Task<RefreshToken> GetRefreshTokenByAccessToken(string accessToken)
        {
            var user = await GetUserByAccessToken(accessToken);
            if (user == null)
            {
                return null;
            }
            else
            {
                if (user.RefreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    user.RefreshToken = GenerateRefreshToken();
                }
                return user.RefreshToken;
            }
        }
        //--------------------------------------------------------------------------------------


        //=================================== PASSWORD =========================================
        // tao ra code va gui den email, code nay dung de confirm
        public async Task<string> ResetPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));
                var callbackUrl = ConstStrings.BASE_URL_WEB_CLIENT + "/Account/ForgotPasswordConfirm/?ResetCode=" + resetCode + "&Email=" + model.Email;
                await _emailSender.SendEmailAsync(model.Email, "Reset password confirmation", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                return resetCode;
            }
            return null;
        }
        // dung code nhan duoc trong mail va confirm
        public async Task<bool> ResetPasswordConfirm(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.ResetCode));
                var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
                return true;
            }
            return false;
        }
        public async Task<AppUser> AddPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!await _userManager.HasPasswordAsync(user))
                    await _userManager.AddPasswordAsync(user, model.Password);
            }
            return user;
        }
        public async Task<string> ChangePassword(int userID, ChangePasswordModel model)
        {
            var user = await _db.Users.FindAsync(userID);
            if (user == null)
            {
                return "Tài khoản này không tồn tại";
            }
            if (user.isActive == false)
            {
                return "Tài khoản này đã bị khoá";
            }
            var IsPasswordOK = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (IsPasswordOK)
            {
                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                return null;
            }
            else
            {
                return "Mật khẩu cũ không đúng";
            }
        }
        //--------------------------------------------------------------------------------------

    }
}
