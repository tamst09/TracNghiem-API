using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using System.Text.Json;
using System;
using TN.ViewModels.Common;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.FacebookAuth;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly  IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userService.GetAll());
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

        // GET: api/Users/detail/5
        [HttpGet("detail/{id}")]
        public async Task<ActionResult<AppUser>> GetUserDetails(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel request)
        {
            var result = await _userService.Authenticate(request);
            if (result == null)
            {
                return BadRequest("Invalid username or password");
            }
            return Ok(result);
        }

        // POST: api/Users/register
        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> RegisterUser([FromBody] RegisterModel request)
        {
            var result = await _userService.Register(request);
            if (result == null) return BadRequest("Unsuccesfully register");
            return Ok(result);
        }

        // POST: api/Users/getresetcode
        [HttpPost("getresetcode")]
        public async Task<IActionResult> forgotPassword([FromBody] ForgotPasswordModel model)
        {
            var resultStringCode = await _userService.ResetPassword(model);
            if(resultStringCode!=null)
                return Ok(resultStringCode);
            return NotFound("User is not found");
        }

        // POST: api/Users/resetpass
        [HttpPost("resetpass")]
        public async Task<IActionResult> resetPassword([FromBody] ResetPasswordModel model)
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
        public async Task<ActionResult<AppUser>> PutUser(int id, [FromBody] AppUser user)
        {
            return await _userService.EditUserInfo(id, user);
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("CreateUser")]
        public async Task<ActionResult<JwtResponse>> PostUser([FromBody]RegisterModel user)
        {
            return await _userService.Register(user);
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            return await _userService.DeleteUser(id);
        }

        [HttpPost("createfacebookuser")]
        public async Task<ActionResult<CreateFacebookUserResult>> CreateFBUser([FromBody]string accesstoken)
        {
            var loginUser = await _userService.GetUserWithFacebookToken(accesstoken);
            return Ok(loginUser);
        }
    }
}
