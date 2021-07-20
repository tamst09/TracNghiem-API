using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.Common;


namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var lstUser = await _userService.GetAll();
            return Ok(new ResponseBase<List<AppUser>>(data: lstUser));
        }

        // POST: api/Users/Paged
        [HttpPost("Paged")]
        public async Task<IActionResult> GetUsersPaged(UserPagingRequest model)
        {
            var result = await _userService.GetListUserPaged(model);
            if (result == null)
            {
                return Ok(new ResponseBase<PagedResult<UserViewModel>>(success: false, msg:"Out of index.", data: result));
            }
            return Ok(new ResponseBase<PagedResult<UserViewModel>>(data: result));
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetByID(id);
            if (user == null)
            {
                return Ok(new ResponseBase<UserViewModel>(success: false, msg: "User not found.", data: user));
            }
            return Ok(new ResponseBase<UserViewModel>(data: user));
        }

        // GET: api/Users/Count
        [HttpGet("Count")]
        public async Task<IActionResult> CountUser()
        {
            NumberUserInfo countUser = await _userService.CountUser();
            return Ok(new ResponseBase<NumberUserInfo>(data: countUser));
        }

        // GET: api/GetStatus/5
        [HttpGet("GetStatus/{id}")]
        public async Task<IActionResult> GetStatus(int id)
        {
            var user = await _userService.GetByID(id);
            if (user != null && user.isActive)
            {
                return Ok(new ResponseBase<UserViewModel>(data: user));
            }
            else
            {
                return Ok(new ResponseBase(msg: "User not found or is locked", success: false));
            }
        }

        // POST: api/Users/CreateUser
        [HttpPost("CreateUser")]
        public async Task<IActionResult> AddUser([FromBody] RegisterModel user)
        {
            var result = await _userService.AddUser(user);
            return Ok(new ResponseBase(success: result, msg: result ? "Success." : "Failed."));
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleteResult = await _userService.DeleteUser(id);
            if (deleteResult)
            {
                return Ok(new ResponseBase(msg: "User not found", success: false));
            }
            return Ok(new ResponseBase());
        }

        [HttpPost("LockUser/{id}")]
        public async Task<IActionResult> LockUser(int id)
        {
            var lockUserResult = await _userService.DeleteUser(id);
            if (lockUserResult)
            {
                return Ok(new ResponseBase(msg: $"User with id {id} was locked"));
            }
            return Ok(new ResponseBase(msg: "Failed.", success: false));
        }

        [HttpPost("RestoreUser/{id}")]
        public async Task<IActionResult> RestoreUser(int id)
        {
            var result = await _userService.RestoreUser(id);
            if (result)
            {
                return Ok(new ResponseBase(msg: $"User with id {id} was unlocked"));
            }
            return Ok(new ResponseBase(msg: "Failed.", success: false));
        }
    }
}
