using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using System;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.Common;

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
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userService.GetAll());
        }

        [HttpPost("Paged")]
        [Authorize("admin")]
        public async Task<ActionResult<PagedResult<UserViewModel>>> GetUsersPaged(UserPagingRequest model)
        {
            var result = await _userService.GetListUserPaged(model);
            if (result == null)
            {
                return BadRequest("Out of index");
            }
            return Ok(result);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // GET: api/Users/GetNumber
        [HttpGet("GetNumber")]
        [Authorize("admin")]
        public async Task<ActionResult<NumberUserInfo>> GetNumberUser()
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
            return Ok(response);
        }

        // GET: api/GetStatus/5
        [HttpGet("GetStatus/{id}")]
        public async Task<ActionResult<AppUser>> GetStatus(int id)
        {
            var user = await _userService.GetByID(id);
            if (user!=null && user.isActive)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        // GET: api/Users/Detail/5
        [HttpGet("Detail/{id}")]
        public async Task<ActionResult<AppUser>> GetUserDetails(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel request)
        {
            var result = await _userService.Login(request);
            if (result == null || result.Error != null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST: api/Users/register
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<JwtResponse>> RegisterUser([FromBody] RegisterModel request)
        {
            var result = await _userService.Register(request);
            if (!string.IsNullOrEmpty(result.Error))
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST: api/Users/GetResetCode
        [HttpPost("GetResetCode")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var resultStringCode = await _userService.ResetPassword(model);
            if(resultStringCode!=null)
                return Ok(resultStringCode);
            return NotFound("User is not found");
        }

        // POST: api/Users/ResetPass
        [HttpPost("ResetPass")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _userService.ResetPasswordConfirm(model);
            if (result=="OK")
            {
                return Ok("Password Changed");
            }
            return NotFound("user not found");
        }


        //POST: api/Users/RefreshToken
        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshAccessTokenRequest refreshRequest)
        {
            string newAccessToken = await _userService.GetNewAccessToken(refreshRequest);
            if (!String.IsNullOrEmpty(newAccessToken))
            {
                return Ok(new { access_token = newAccessToken });
            }
            return BadRequest("User not found or Refresh Token is invalid");
        }


        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult<AppUser>> PutUser(int id, [FromBody] UserViewModel user)
        {
            var u = await _userService.EditUserInfo(id, user);
            return Ok(u);
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("CreateUser")]
        public async Task<ActionResult<JwtResponse>> PostUser([FromBody]RegisterModel user)
        {
            var u = await _userService.Register(user);
            return Ok(u);
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        [Authorize("admin")]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            return await _userService.DeleteUser(id);
        }

        [HttpPost("LoginFb")]
        [AllowAnonymous]
        public async Task<ActionResult<JwtResponse>> LoginFacebook([FromBody]string accesstoken)
        {
            var loginUser = await _userService.LoginWithFacebookToken(accesstoken);
            return Ok(loginUser);
        }

        [HttpPut("AddPassword")]
        [AllowAnonymous]
        public async Task<ActionResult<AppUser>> AddPasswordForNewUser(ResetPasswordModel model)
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
        public async Task<ActionResult> LockUser(int id)
        {
            var lockUserResult = await _userService.DeleteUser(id);
            if (lockUserResult)
            {
                return Ok("Locked user - UID: "+id);
            }
            else
            {
                return BadRequest("Locked user failed");
            }
        }

        [HttpPost("RestoreUser/{id}")]
        [Authorize("admin")]
        public async Task<ActionResult> RestoreUser(int id)
        {
            var lockUserResult = await _userService.RestoreUser(id);
            if (lockUserResult)
            {
                return Ok("Unlocked user - UID: " + id);
            }
            else
            {
                return BadRequest("Unlocked user failed");
            }
        }
    }
}
