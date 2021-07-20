using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace TN.BackendAPI.Controllers
{
    [Route("api/Users")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var result = await _authService.Login(request);
            return Ok(result);
        }

        [HttpPost("LoginFb")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginFacebook([FromBody] string accesstoken)
        {
            var jwt = await _authService.LoginWithFacebookToken(accesstoken);
            if (jwt == null)
                return Ok(new ResponseBase<JwtResponse>(data: null, success: false, msg: "Invalid token"));
            return Ok(new ResponseBase<JwtResponse>(data: jwt));
        }

        [HttpPost("LoginGG")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginGoogle([FromBody] string token)
        {
            Payload payload;
            try
            {
                payload = await ValidateAsync(token, new ValidationSettings
                {
                    Audience = new[] { "104872694801-4vdhqd2c8e32j65oqd50idd43dh08teo.apps.googleusercontent.com" }
                });
                var ggID = payload.Subject;
                var email = payload.Email;
                var name = payload.Name;
                var avatar = payload.Picture;

                var jwt = await _authService.LoginWithGoogleToken(token, email, name, avatar, ggID);
                if (jwt == null)
                    return Ok(new ResponseBase<JwtResponse>(success: false, msg:"Invalid Token", data: null));
                return Ok(new ResponseBase<JwtResponse>(data: jwt));
            }
            catch (Exception e)
            {
                return Ok(new ResponseBase<JwtResponse>(success: false, msg: e.Message, data: null));
            }
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel request)
        {
            var result = await _authService.Register(request);
            return Ok(result);
        }

        // POST: api/Users/GetResetCode
        [HttpPost("GetResetCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var resultStringCode = await _authService.ResetPassword(model);
            if (resultStringCode != null)
                return Ok(new ResponseBase<string>(data: resultStringCode));
            return Ok(new ResponseBase<string>(success: false, msg: "User not found", data: "User not found"));
        }

        // POST: api/Users/ResetPass
        [HttpPost("ResetPass")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _authService.ResetPasswordConfirm(model);
            if (result)
            {
                return Ok(new ResponseBase<string>(data: "Password changed"));
            }
            return Ok(new ResponseBase<string>(success: false, msg: "User not found", data: "User not found"));
        }

        // POST: api/Users/ChangePass?userId=1
        [HttpPost("ChangePass")]
        public async Task<IActionResult> ChangePassword([FromQuery] int userId, [FromBody] ChangePasswordModel model)
        {
            var result = await _authService.ChangePassword(userId, model);
            return Ok(new ResponseBase(msg: result));
        }

        //POST: api/Users/RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshAccessTokenRequest refreshRequest)
        {
            string newAccessToken = await _authService.GenerateAccessTokenWithRefressToken(refreshRequest);
            if (!string.IsNullOrWhiteSpace(newAccessToken))
            {
                return Ok(new ResponseBase<string>(data: newAccessToken));
            }
            return Ok(new ResponseBase(msg: "User not found or Refresh Token is invalid"));
        }

        //POST: api/Users/GetRefreshToken
        [HttpPost("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshTokenByAccessToken([FromBody] RefreshAccessTokenRequest accessToken)
        {
            var refreshToken = await _authService.GetRefreshTokenByAccessToken(accessToken.AccessToken);
            if (refreshToken != null)
                return Ok(new ResponseBase<RefreshToken>(data: refreshToken));
            return Ok(new ResponseBase<RefreshToken>(data: null, msg: "Invalid access token.", success: false));
        }

        //GET: api/Users/Profile
        [HttpGet("Profile")]
        public async Task<IActionResult> GetUserInfoByToken()
        {
            var userId = GetCurrentUserId();
            if (userId == -1)
            {
                return Ok(new ResponseBase<UserInfo>(success: false, msg: "Some error happened.", data: null));
            }
            return Ok(await _authService.GetProfile(userId));
        }

        // POST: api/Users/UpdateProfile
        [HttpPut]
        public async Task<IActionResult> EditProfile([FromBody] UserViewModel user)
        {
            if (User.IsInRole("admin"))
            {
                var editResult = await _authService.UpdateProfile(user);
                return Ok(editResult);
            }
            else
            {
                if (user.Id != GetCurrentUserId())
                {
                    return Forbid();
                }
                var editResult = await _authService.UpdateProfile(user);
                return Ok(editResult);
            }
            
        }

        [HttpPut("AddPassword")]
        public async Task<IActionResult> AddPassword(ResetPasswordModel model)
        {
            if(model.Email != GetCurrentUserEmail())
            {
                return Forbid();
            }
            var user = await _authService.AddPassword(model);
            if (user == null)
            {
                return Ok(new ResponseBase(success: false, msg: "User not found."));
            }
            return Ok(new ResponseBase());
        }

        private int GetCurrentUserId()
        {
            var check = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (check)
                return userId;
            return -1;
        }

        private string GetCurrentUserEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }
    }
}
