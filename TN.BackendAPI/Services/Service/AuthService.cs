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
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace TN.BackendAPI.Services.Service
{
    public class AuthService : IAuthService
    {
        private readonly TNDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        private readonly IFacebookAuth _facebookAuth;
        private readonly IEmailSender _emailSender;

        public AuthService(TNDbContext db, 
            UserManager<AppUser> userManager, 
            IConfiguration config, 
            IFacebookAuth facebookAuth, 
            IEmailSender emailSender)
        {
            _db = db;
            _userManager = userManager;
            _config = config;
            _facebookAuth = facebookAuth;
            _emailSender = emailSender;
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
        
        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken();
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken.Value = Convert.ToBase64String(randomNumber);
            }
            //thoi han token
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
            return refreshToken;
        }

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
                        Refresh_Token = userLogin.RefreshToken.Value
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
                        Refresh_Token = userLogin.RefreshToken.Value
                    },
                    success = true,
                    msg = "Login successfully"
                };
            }
        }

        public async Task<ResponseBase<JwtResponse>> Register(RegisterModel model)
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
            try
            {
                await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "user");
                RefreshToken refresh_token = GenerateRefreshToken();
                user.RefreshToken = GenerateRefreshToken();
                await _db.SaveChangesAsync();
                var jwt = new JwtResponse() { Access_Token = GenerateAccessToken(user), Refresh_Token = refresh_token.Value };
                return new ResponseBase<JwtResponse>()
                {
                    data = jwt,
                    success = true,
                    msg = "New user is created"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<JwtResponse>()
                {
                    data = null,
                    success = false,
                    msg = e.Message
                };
            }
        }

        public async Task<ResponseBase<string>> GenerateAccessTokenWithRefreshToken(RefreshAccessTokenRequest refreshRequest)
        {
            AppUser user = await GetUserByAccessToken(refreshRequest.AccessToken);
            if (user != null)
            {
                if (ValidateRefreshToken(user, refreshRequest.RefreshToken))
                {
                    var u = user.RefreshToken.ExpiryDate.Subtract(DateTime.UtcNow);
                    if (u.TotalDays < 2)
                    {
                        user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                    }
                    return new ResponseBase<string>()
                    {
                        success = true,
                        data = GenerateAccessToken(user),
                        msg = "New access token is generated"
                    };
                }
                else
                {
                    return new ResponseBase<string>()
                    {
                        success = false,
                        data = null,
                        msg = "Invalid refresh token"
                    };
                }
            }
            return new ResponseBase<string>()
            {
                success = false,
                data = null,
                msg = "User not found"
            };
        }
        
        public async Task<ResponseBase<RefreshToken>> GetRefreshTokenByAccessToken(string accessToken)
        {
            var user = await GetUserByAccessToken(accessToken);
            if (user != null)
            {
                if (user.RefreshToken.ExpiryDate < DateTime.UtcNow)
                {
                    user.RefreshToken = GenerateRefreshToken();
                }
                return new ResponseBase<RefreshToken>()
                {
                    success = true,
                    data = user.RefreshToken,
                    msg = "Success"
                };
            }
            return new ResponseBase<RefreshToken>()
            {
                success = false,
                data = null,
                msg = "User not found"
            };
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
                    return await _db.Users.Where(u => u.UserName == userName && u.isActive == true).Include(u => u.RefreshToken).FirstOrDefaultAsync();
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        public bool ValidateRefreshToken(AppUser user, string refreshToken)
        {

            RefreshToken refreshTokenUser = _db.RefreshTokens.Where(rt => rt.Value == refreshToken).FirstOrDefault();
            if (refreshTokenUser != null && refreshTokenUser.User == user && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }

        public async Task<ResponseBase<JwtResponse>> LoginWithFacebookToken(string fbToken)
        {
            // validate facebook access token
            var validator = await _facebookAuth.ValidateAccessTokenAsync(fbToken);
            if (!validator.Data.IsValid)
            {
                return new ResponseBase<JwtResponse>()
                {
                    success = false,
                    data = null,
                    msg = "Invalid fb token"
                };
            }
            // get fb user info
            try
            {
                var user = new AppUser();
                var userInfo = await _facebookAuth.GetUserInfoAsync(fbToken);
                // find user by social id
                var userToken = await _db.UserTokens.FirstOrDefaultAsync(p => p.LoginProvider == "Facebook" && p.Name == "fbID" && p.Value == userInfo.SocialID);
                if(userToken != null)
                {
                    user = await _userManager.FindByIdAsync(userToken.UserId.ToString());
                }
                else
                {
                    user = new AppUser()
                    {
                        Name = userInfo.FirstName + " " + userInfo.LastName,
                        Email = userInfo.Email,
                        UserName = userInfo.FirstName.ToLower() + " " + userInfo.LastName.ToLower(),
                        isActive = true,
                        Avatar = "/images/cover/user/default_avatar.png"
                    };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Facebook", userToken.UserId.ToString(), "Facebook"));
                    await _userManager.AddToRoleAsync(user, "user");
                    await _userManager.SetAuthenticationTokenAsync(user, "Facebook", "fbID", userInfo.SocialID);

                }
                // generate token then return
                string access_token = GenerateAccessToken(user);
                if (user.RefreshTokenValue != null)
                {
                    user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    RefreshToken refreshToken = new RefreshToken();
                    refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    _db.RefreshTokens.Add(refreshToken);
                    await _db.SaveChangesAsync();
                }
                // create jwt
                JwtResponse token = new JwtResponse()
                {
                    Access_Token = access_token,
                    Refresh_Token = user.RefreshToken.Value
                };
                return new ResponseBase<JwtResponse>()
                {
                    data = token,
                    msg = "Logged in with facebook",
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<JwtResponse>()
                {
                    success = false,
                    data = null,
                    msg = e.Message
                };
            }
        }

        public async Task<ResponseBase<JwtResponse>> LoginWithGoogleToken(string token)
        {
            Payload payload;
            string ggID = null;
            string email = null;
            string name = null;
            string avatar = null;
            try
            {
                payload = await ValidateAsync(token, new ValidationSettings
                {
                    Audience = new[] { "104872694801-4vdhqd2c8e32j65oqd50idd43dh08teo.apps.googleusercontent.com" }
                });
                ggID = payload.Subject;
                email = payload.Email;
                name = payload.Name;
                avatar = payload.Picture;
            }
            catch (Exception e)
            {
                return new ResponseBase<JwtResponse>()
                {
                    success = false,
                    data = null,
                    msg = e.Message
                };
            }

            var user = await _db.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new AppUser()
                {
                    Name = name,
                    Email = email,
                    UserName = name.Trim().ToLower(),
                    isActive = true,
                    Avatar = avatar
                };
                try
                {
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", ggID, "Google"));
                    await _userManager.AddToRoleAsync(user, "user");
                    await _userManager.SetAuthenticationTokenAsync(user, "Google", "ggID", ggID);
                }
                catch (Exception e)
                {
                    return new ResponseBase<JwtResponse>()
                    {
                        success = false,
                        data = null,
                        msg = e.Message
                    };
                }
            }
            string access_token = GenerateAccessToken(user);
            try
            {
                if (user.RefreshTokenValue != null)
                {
                    user.RefreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    RefreshToken refreshToken = new RefreshToken();
                    refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    _db.RefreshTokens.Add(refreshToken);
                    await _db.SaveChangesAsync();
                }
                var jwt = new JwtResponse()
                {
                    Access_Token = access_token,
                    Refresh_Token = user.RefreshToken.Value
                };
                return new ResponseBase<JwtResponse>()
                {
                    success = true,
                    data = jwt,
                    msg = "Logged in with google"
                };
            }
            catch (Exception e)
            {
                return new ResponseBase<JwtResponse>()
                {
                    success = false,
                    data = null,
                    msg = e.Message
                };
            }

        }

        public async Task<ResponseBase<bool>> AddPassword(AddPasswordModel model)
        {
            if(model.Password != model.ConfirmPassword)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = "Password and confirm password do not match"
                };
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                try
                {
                    if (!await _userManager.HasPasswordAsync(user))
                        await _userManager.AddPasswordAsync(user, model.Password);
                    return new ResponseBase<bool>()
                    {
                        success = true,
                        data = true,
                        msg = "New password is added"
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
            return new ResponseBase<bool>()
            {
                success = false,
                data = false,
                msg = "User email not found"
            };
        }

        public async Task<ResponseBase<bool>> ChangePassword(int userID, ChangePasswordModel model)
        {
            var user = await _db.Users.FindAsync(userID);
            if (user == null)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = "User not found"
                };
            }
            if (user.isActive == false)
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = "This user is locked"
                };
            }
            var IsPasswordOK = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (IsPasswordOK)
            {
                try
                {
                    await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    return new ResponseBase<bool>()
                    {
                        success = true,
                        data = true,
                        msg = "Password changed"
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
            else
            {
                return new ResponseBase<bool>()
                {
                    success = false,
                    data = false,
                    msg = "Wrong password"
                };
            }
        }

        public async Task<ResponseBase<string>> SendResetPasswordCode(ForgotPasswordModel model)
        {
            // send code to email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                resetCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetCode));
                var callbackUrl = ConstStrings.BASE_URL_WEB_CLIENT + "/Account/ForgotPasswordConfirm/?ResetCode=" + resetCode + "&Email=" + model.Email;
                await _emailSender.SendEmailAsync(model.Email, "Reset password confirmation", $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                return new ResponseBase<string>()
                {
                    success = true,
                    data = resetCode,
                    msg = "A code is sent to "+model.Email
                };
            }
            return new ResponseBase<string>()
            {
                success = false,
                data = null,
                msg = "User not found"
            };
        }

        public async Task<ResponseBase<bool>> ResetPassword(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.ResetCode));
                try
                {
                    await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);
                    return new ResponseBase<bool>()
                    {
                        success = true,
                        data = true,
                        msg = "User not found"
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
            return new ResponseBase<bool>()
            {
                success = false,
                data = false,
                msg = "User not found"
            };
        }
    }
}
