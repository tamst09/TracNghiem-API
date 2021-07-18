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
    public class AuthService : IAuthService
    {
        private readonly TNDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IFacebookAuth _facebookAuth;
        private readonly IEmailSender _emailSender;
        public AuthService(
            TNDbContext context,
            UserManager<AppUser> userManager,
            IConfiguration config, IFacebookAuth facebookAuth, IEmailSender emailSender)
        {
            _dbContext = context;
            _userManager = userManager;
            _config = config;
            _facebookAuth = facebookAuth;
            _emailSender = emailSender;
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
            var user = await _dbContext.Users.FindAsync(userID);
            if (user == null)
            {
                return "Invalid account.";
            }
            if (user.isActive == false)
            {
                return "This account was locked.";
            }
            var IsPasswordOK = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (IsPasswordOK)
            {
                await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                return "Password changed.";
            }
            else
            {
                return "Invalid password.";
            }
        }

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
                    return await _dbContext.Users.Where(u => u.UserName == userName && u.isActive == true).Include(u => u.RefreshToken).FirstOrDefaultAsync();
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ResponseBase<JwtResponse>> Login(LoginModel model)
        {
            var userlogin = await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.UserName == model.UserName);

            if (userlogin == null)
                return new ResponseBase<JwtResponse>(success: false, msg: "Invalid account", data: null);

            if (userlogin.isActive == false)
                return new ResponseBase<JwtResponse>(success: false, msg: "This account was locked", data: null);

            var passwordvalid = await _userManager.CheckPasswordAsync(userlogin, model.Password);
            if (!passwordvalid)
                return new ResponseBase<JwtResponse>(success: false, msg: "Wrong password", data: null);

            // generate new access_token
            string access_token = GenerateAccessToken(userlogin);
            // access_token is available
            if (userlogin.RefreshTokenValue != null)
            {
                userlogin.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                await _dbContext.SaveChangesAsync();
                return new ResponseBase<JwtResponse>(msg: "Login successfully.", data: new JwtResponse()
                {
                    Access_Token = access_token,
                    Refresh_Token = userlogin.RefreshToken.Token
                });
            }
            // new user
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                userlogin.RefreshToken = refreshToken;
                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();
                return new ResponseBase<JwtResponse>(msg: "Login successfully.", data: new JwtResponse()
                {
                    Access_Token = access_token,
                    Refresh_Token = userlogin.RefreshToken.Token
                });
            }
        }

        public async Task<JwtResponse> LoginWithFacebookToken(string accessToken)
        {
            var validatedToken = await _facebookAuth.ValidateAccessTokenAsync(accessToken);
            if (!validatedToken.Data.IsValid)
            {
                return null;
            }
            bool isNewUser = true;
            var FbUserInfo = await _facebookAuth.GetUserInfoAsync(accessToken);
            if (string.IsNullOrEmpty(FbUserInfo.Email))
            {
                FbUserInfo.Email = FbUserInfo.FirstName.ToLower() + "_" + FbUserInfo.LastName.ToLower();
            }
            var user = await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.UserName == FbUserInfo.Email || u.Email == FbUserInfo.Email);

            if (user == null)
            {
                var findFacebookUID = await _dbContext.UserTokens.FirstOrDefaultAsync(p => p.LoginProvider == "Facebook" && p.Name == "fbID" && p.Value == FbUserInfo.Id);
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
                    Name = FbUserInfo.FirstName + " " + FbUserInfo.LastName,
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
                await _dbContext.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = user.RefreshToken.Token, isNewLogin = isNewUser };
            }
            // new login info
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = refreshToken.Token, isNewLogin = isNewUser };
            }
        }

        public async Task<JwtResponse> LoginWithGoogleToken(string token, string email, string name, string avatar, string ggID)
        {
            bool isNewUser = true;
            var user = await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                var userGetByGoogleID = await _dbContext.UserTokens.FirstOrDefaultAsync(p => p.LoginProvider == "Google" && p.Name == "ggID" && p.Value == ggID);
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
                await _dbContext.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = user.RefreshToken.Token, isNewLogin = isNewUser };
            }
            // new login info
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();
                return new JwtResponse() { Access_Token = access_token, Refresh_Token = refreshToken.Token, isNewLogin = isNewUser };

            }
        }

        public async Task<ResponseBase<JwtResponse>> Register(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.AvatarURL))
            {
                model.AvatarURL = "/images/cover/user/default_avatar.png";
            }
            var user = new AppUser()
            {
                UserName = model.UserName,
                Name = model.Name,
                Email = model.Email,
                DoB = model.DoB,
                PhoneNumber = model.PhoneNumber,
                Avatar = model.AvatarURL
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                RefreshToken refresh_token = GenerateRefreshToken();
                user.RefreshToken = refresh_token;
                await _dbContext.SaveChangesAsync();
                return new ResponseBase<JwtResponse>(data: new JwtResponse()
                {
                    Access_Token = GenerateAccessToken(user),
                    Refresh_Token = refresh_token.Token
                });
            }
            return new ResponseBase<JwtResponse>(success: false, msg: result.Errors.First().Description, data: null);
        }

        public async Task<AppUser> UpdateProfile(UserViewModel model)
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

        public bool ValidateRefreshToken(AppUser user, string refreshToken)
        {
            RefreshToken refreshTokenUser = _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefault();
            if (refreshTokenUser != null && refreshTokenUser.User == user && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        private string GenerateAccessToken(AppUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Tokens:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
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

        public async Task<ResponseBase<UserInfo>> GetProfile(int userId)
        {
            var user = await _dbContext.Users.Where(u => u.isActive == true).Select(u => new UserInfo()
            {
                UserId = u.Id,
                AvatarUrl = u.Avatar,
                Email = u.Email,
                UserName = u.UserName
            }).FirstOrDefaultAsync();
            if (user == null)
            {
                return new ResponseBase<UserInfo>(success: false, msg: "Invalid user.", data: null);
            }
            return new ResponseBase<UserInfo>(data: user);
        }
    }
}
