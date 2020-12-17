using Microsoft.AspNetCore.Identity;
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
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace TN.Business.Catalog.Implementor
{
    public class UserService : IUserService
    {
        private readonly TNDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;
        public UserService(
            TNDbContext context,
            UserManager<AppUser> userManager,
            IConfiguration config)
        {
            _dbContext = context;
            _userManager = userManager;
            _config = config;
        }

        public async Task<JwtResponse> Register(RegisterModel request)
        {
            var user = new AppUser()
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DoB = request.DoB,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                RefreshToken refresh_token = GenerateRefreshToken();
                user.RefreshToken = refresh_token;
                await _dbContext.SaveChangesAsync();
                return new JwtResponse() { Access_Token = GenerateAccessToken(user), Refresh_Token = refresh_token.Token };
            }
            return null;
        }
        public async Task<IEnumerable<AppUser>> GetAll()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<AppUser> GetByID(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }
        public async Task<JwtResponse> Authenticate(LoginModel request)
        {
            //var user = await _userManager.FindByNameAsync(request.UserName);
            var userlogin = await _dbContext.Users.Include(u => u.RefreshToken).FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (userlogin == null) return null;
            var passwordvalid = await _userManager.CheckPasswordAsync(userlogin, request.Password);
            if (!passwordvalid)
            {
                return null;
            }
            // generate new access_token
            string access_token = GenerateAccessToken(userlogin);
            // access_token is available
            if(userlogin.RefreshTokenValue != null)
            {
                userlogin.RefreshToken.ExpiryDate = DateTime.Now.AddDays(7);
                await _dbContext.SaveChangesAsync();
                return (new JwtResponse() { Access_Token = access_token, Refresh_Token = userlogin.RefreshToken.Token }); // return access_token with refresh_token
            }
            // new user
            else
            {
                RefreshToken refreshToken = new RefreshToken();
                refreshToken = GenerateRefreshToken();
                userlogin.RefreshToken = refreshToken;
                _dbContext.RefreshTokens.Add(refreshToken);
                await _dbContext.SaveChangesAsync();
                return (new JwtResponse() { Access_Token = access_token, Refresh_Token = refreshToken.Token }); // return access_token with refresh_token
            }
        }
        private string GenerateAccessToken(AppUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Tokens:SecretKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName+" "+user.LastName),
                    new Claim(ClaimTypes.Role, string.Join(";",roles))
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
        public async Task<AppUser> EditUserInfo(int id, AppUser user)
        {
            _dbContext.Entry(user).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return user;
        }
        public async Task<bool> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        // tao ra code va gui den email, code nay dung de confirm
        public async Task<string> ResetPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user != null)
            {
                string resetCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                return resetCode;
            }
            return null;
        }
        private bool UserExists(int id)
        {
            return _dbContext.Users.Any(e => e.Id == id);
        }
        // dung code nhan duoc trong mail va doi mat khau
        public async Task<string> ResetPasswordConfirm(ResetPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                return "OK";
            }
            return null;
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
                    return await _dbContext.Users.Where(u => u.UserName == userName).Include(u => u.RefreshToken).FirstOrDefaultAsync();
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
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
            refreshToken.ExpiryDate = DateTime.Now.AddDays(7);
            return refreshToken;
        }
        // Xac thuc han su dung refreshToken
        public bool ValidateRefreshToken(AppUser user, string refreshToken)
        {

            RefreshToken refreshTokenUser = _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefault();
            if (refreshTokenUser != null && refreshTokenUser.User == user && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }
        //gia han access_token va tra ve client
        public async Task<string> GetNewAccessToken(RefreshAccessTokenRequest refreshRequest)
        {
            AppUser user = await GetUserByAccessToken(refreshRequest.AccessToken);
            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                var u = user.RefreshToken.ExpiryDate.Subtract(DateTime.Now);
                if (u.TotalDays < 2)
                {
                    user.RefreshToken.ExpiryDate = DateTime.Now.AddDays(7);
                }
                return GenerateAccessToken(user);
            }
            return null;
        }
    }
}
