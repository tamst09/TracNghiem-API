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
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        public UsersController(IUserService userService, IAuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize("admin")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetAll());
        }

        // POST: api/Users/Paged
        [HttpPost("Paged")]
        [Authorize("admin")]
        public async Task<IActionResult> GetUsersPaged(UserPagingRequest model)
        {
            return Ok(await _userService.GetListUserPaged(model));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(await _userService.GetByID(id));
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
            response.TotalUser = allUsers.data.Count;
            foreach (var u in allUsers.data)
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
            return Ok(await _userService.GetByID(id));
        }

        // GET: api/Users/Detail/5
        [HttpGet("Detail/{id}")]
        public async Task<IActionResult> GetUserDetails(int id)
        {
            return Ok(await _userService.GetByID(id));
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            return Ok(await _authService.Login(request));
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel request)
        {
            return Ok(await _authService.Register(request));
        }

        // POST: api/Users/GetResetCode
        [HttpPost("GetResetCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            return Ok(await _authService.SendResetPasswordCode(model));
        }

        // POST: api/Users/ResetPass
        [HttpPost("ResetPass")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            return Ok(await _authService.ResetPassword(model));
        }

        // POST: api/Users/ChangePass/1
        [HttpPost("ChangePass/{userID}")]
        public async Task<IActionResult> ChangePassword(int userID, [FromBody] ChangePasswordModel model)
        {
            return Ok(await _authService.ChangePassword(userID, model));
        }

        //POST: api/Users/RefreshToken
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshAccessTokenRequest refreshRequest)
        {
            return Ok(await _authService.GenerateAccessTokenWithRefreshToken(refreshRequest));
        }

        //POST: api/Users/GetRefreshToken
        [HttpPost("GetRefreshToken")]
        public async Task<IActionResult> GetRefreshTokenByAccessToken([FromBody] RefreshAccessTokenRequest accessToken)
        {
            return Ok(await _authService.GetRefreshTokenByAccessToken(accessToken.AccessToken));
        }

        // PUT: api/Users/EditUser
        [HttpPost("EditUser")]
        [Authorize("admin")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel model)
        {
            return Ok(await _userService.EditUserInfo(model.Id, model));
        }

        // PUT: api/Users/UpdateProfile/5
        [HttpPut("UpdateProfile/{userID}")]
        public async Task<IActionResult> EditProfile(int userID, [FromBody] UserViewModel user)
        {
            return Ok(await _userService.EditProfile(userID, user));
        }

        // POST: api/Users/CreateUser
        [HttpPost("CreateUser")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PostUser([FromBody]RegisterModel user)
        {
            return Ok(await _authService.Register(user));
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            return Ok(await _userService.DeleteUser(id));
        }

        [HttpPost("LoginFb")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginFacebook([FromBody]string accesstoken)
        {
            return Ok(await _authService.LoginWithFacebookToken(accesstoken));
        }

        [HttpPost("LoginGG")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginGoogle([FromBody]string token)
        {
            return Ok(await _authService.LoginWithGoogleToken(token));
        }

        [HttpPut("AddPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> AddPasswordForNewUser(AddPasswordModel model)
        {
            return Ok(await _authService.AddPassword(model));
        }

        [HttpPost("LockUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> LockUser(int id)
        {
            return Ok(await _userService.DeleteUser(id));   
        }

        [HttpPost("RestoreUser/{id}")]
        [Authorize("admin")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            return Ok(await _userService.RestoreUser(id));
        }
    }
}
