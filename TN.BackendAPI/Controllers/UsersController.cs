using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using System;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.Common;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly  IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize("admin")]
        public async Task<IActionResult> GetUsers()
        {
            var lstUser = await _userService.GetAll();
            return Ok(new ResponseBase<List<AppUser>>() { data=lstUser });
        }

        // POST: api/Users/Paged
        [HttpPost("Paged")]
        [Authorize("admin")]
        public async Task<IActionResult> GetUsersPaged(UserPagingRequest model)
        {
            var result = await _userService.GetListUserPaged(model);
            if (result == null)
            {
                return Ok(new ResponseBase<PagedResult<UserViewModel>>() { msg="Out of index"} );
            }
            return Ok(new ResponseBase<PagedResult<UserViewModel>>(){ data = result });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return Ok(new ResponseBase<AppUser>() { msg = "User not found" });
            }
            return Ok(new ResponseBase<AppUser>() { data = user });
        }

        // GET: api/Users/GetNumber
        [HttpGet("GetNumber")]
        [Authorize("admin")]
        public async Task<IActionResult> GetNumberUser()
        {
            NumberUserInfo response = new NumberUserInfo();
            response.TotalActiveUser = 0;
            response.TotalInactiveUser = 0;
            var allUsers = await _userService.GetAll();
            response.TotalUser = allUsers.Count;
            foreach (var u in allUsers)
            {
                if (u.isActive)
                    response.TotalActiveUser++;
            }
            response.TotalInactiveUser = response.TotalUser - response.TotalActiveUser;
            return Ok(new ResponseBase<NumberUserInfo>() { data = response });
        }

        // GET: api/GetStatus/5
        [HttpGet("GetStatus/{id}")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var user = await _userService.GetByID(id);
            if (user!=null && user.isActive)
            {
                return Ok(new ResponseBase<AppUser>() { data = user });
            }
            else
            {
                return Ok(new ResponseBase<AppUser>() { msg = "User not found or is locked" });
            }
        }

        // GET: api/Users/Detail/5
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return Ok(new ResponseBase<AppUser>() { msg = "User not found" });
            }
            return Ok(new ResponseBase<AppUser>() { data = user });
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var result = await _userService.Login(request);
            if (result == null || result.Error != null)
            {
                return Ok(new ResponseBase<JwtResponse>() { msg = result.Error });
            }
            return Ok(new ResponseBase<JwtResponse>() { data = result });
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel request)
        {
            var result = await _userService.Register(request);
            if (!string.IsNullOrEmpty(result.Error))
            {
                return Ok(new ResponseBase<JwtResponse>() { msg = result.Error });
            }
            return Ok(new ResponseBase<JwtResponse>() { data = result });
        }

        // POST: api/Users/GetResetCode
        [HttpPost("GetResetCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var resultStringCode = await _userService.ResetPassword(model);
            if(resultStringCode!=null)
                return Ok(new ResponseBase<string>() { data = resultStringCode });
            return Ok(new ResponseBase<string>() { msg = "User not found" });
        }

        // POST: api/Users/ResetPass
        [HttpPost("ResetPass")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _userService.ResetPasswordConfirm(model);
            if (result)
            {
                return Ok(new ResponseBase<string>() { data = "Password changed" });
            }
            return Ok(new ResponseBase<string>() { msg = "User not found" });
        }

        // POST: api/Users/ChangePass/1
        [HttpPost("ChangePass/{userID}")]
        public async Task<IActionResult> ChangePassword(int userID, [FromBody] ChangePasswordModel model)
        {
            var result = await _userService.ChangePassword(userID, model);
            return Ok(new ResponseBase<string>() { msg = result });
        }

        //POST: api/Users/RefreshToken
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshAccessTokenRequest refreshRequest)
        {
            string newAccessToken = await _userService.GenerateAccessTokenWithRefressToken(refreshRequest);
            if (!String.IsNullOrEmpty(newAccessToken))
            {
                return Ok(new ResponseBase<string>() { data = newAccessToken } );
            }
            return Ok(new ResponseBase<string>() { msg = "User not found or Refresh Token is invalid" });
        }

        //POST: api/Users/GetRefreshToken
        [HttpPost("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshTokenByAccessToken([FromBody] RefreshAccessTokenRequest accessToken)
        {
            var refreshToken = await _userService.GetRefreshTokenByAccessToken(accessToken.AccessToken);
            if (refreshToken != null)
                return Ok(new ResponseBase<RefreshToken>() { data = refreshToken });
            return Ok(new ResponseBase<string>() { msg = "Invalid access token" });
        }


        // PUT: api/Users/EditUser
        [HttpPost("EditUser")]
        [Authorize("admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            var u = await _userService.EditUserInfo(model.Id, model);
            if (u == null)
            {
                return Ok(new ResponseBase<AppUser>() { msg = "Invalid user info" });
            }
            return Ok(new ResponseBase<AppUser>() { data = u });
        }

        // PUT: api/Users/UpdateProfile/5
        [HttpPut("UpdateProfile/{userID}")]
        public async Task<IActionResult> EditProfile(int userID, [FromBody] UserViewModel user)
        {
            var u = await _userService.EditProfile(userID, user);
            if (u == null)
            {
                return Ok(new ResponseBase<AppUser>() { msg = "Invalid user info" });
            }
            return Ok(new ResponseBase<AppUser>() { data = u });
        }

        // POST: api/Users/CreateUser
        [HttpPost("CreateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostUser([FromBody]RegisterModel user)
        {
            var u = await _userService.Register(user);
            if (u.Error != null)
            {
                return Ok(new ResponseBase<JwtResponse>() { msg = u.Error });
            }
            return Ok(new ResponseBase<JwtResponse>() { data = u });
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteResult = await _userService.DeleteUser(id);
            if (deleteResult)
            {
                return Ok(new ResponseBase<JwtResponse>() { msg = "User not found" });
            }
            return Ok(new ResponseBase<JwtResponse>() { });
        }

        [HttpPost("LoginFb")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginFacebook([FromBody]string accesstoken)
        {
            var jwt = await _userService.LoginWithFacebookToken(accesstoken);
            if (jwt == null)
                return Ok(new ResponseBase<JwtResponse>() { msg = "Invalid token" });
            return Ok(new ResponseBase<JwtResponse>() { data = jwt });
        }

        [HttpPost("LoginGG")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginGoogle([FromBody]string token)
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

                var jwt = await _userService.LoginWithGoogleToken(token ,email, name, avatar, ggID);
                if (jwt == null)
                    return Ok(new ResponseBase<JwtResponse>() { msg = "Error" });
                return Ok(new ResponseBase<JwtResponse>() { data = jwt });
            }
            catch (Exception e)
            {
                return Ok(new ResponseBase<JwtResponse>() { msg = e.Message });
            }
        }

        [HttpPut("AddPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPasswordForNewUser(ResetPasswordModel model)
        {
            var user = await _userService.AddPassword(model);
            if (user == null)
            {
                return BadRequest();
            }
            return Ok(user);
        }

        [HttpPost("LockUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> LockUser(int id)
        {
            var lockUserResult = await _userService.DeleteUser(id);
            if (lockUserResult)
            {
                return Ok(new ResponseBase<string>() { data = "Locked user - UID: " + id });
            }
            else
            {
                return Ok(new ResponseBase<string>() { msg = "Locked user failed" });
            }
        }

        [HttpPost("RestoreUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var lockUserResult = await _userService.RestoreUser(id);
            if (lockUserResult)
            {
                return Ok(new ResponseBase<string>() { data = "Unlocked user - UID: " + id });
            }
            else
            {
                return Ok(new ResponseBase<string>() { msg = "Unlocked user failed" });
            }
        }
    }
}
