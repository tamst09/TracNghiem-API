using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.Entities;
using TN.Data.Model;
using TN.Data.ViewModel;
using TN.ViewModels.Catalog.Users;

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
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return Ok(await _userService.getAll());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await _userService.getByID(id);
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
            var user = await _userService.getByID(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: api/Users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<JwtResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.authenticate(request);
            if (result == "wrong")
            {
                return BadRequest("Wrong username or password");
            }
            else if(result == "null")
            {
                return NotFound("User is not found");
            }            
            return Ok(result);
        }

        // POST: api/Users/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AppUser>> RegisterUser([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.register(request);
            if (result == null) return BadRequest("Unsuccesfully register");
            return Ok(result);
        }

        // POST: api/Users/getresetcode
        [HttpPost("getresetcode")]
        [AllowAnonymous]
        public async Task<IActionResult> forgotPassword([FromBody] ForgotPasswordModel model)
        {
            if(ModelState.IsValid)
            {
                var resultStringCode = await _userService.forgotPassword(model);
                return Ok(resultStringCode);
            }
            return BadRequest(model);
        }

        // POST: api/Users/resetpass
        [HttpPost("resetpass")]
        [AllowAnonymous]
        public async Task<IActionResult> resetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.resetPassword(model);
            
            if (result=="OK")
            {
                return Ok("Password Changed");
            }
            return NotFound("user not found");
        }


        // GET: api/Users
        //[HttpPost("RefreshToken")]
        //public async Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        //{
        //    AppUser user = await GetUserFromAccessToken(refreshRequest.AccessToken);

        //    if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
        //    {
        //        UserWithToken userWithToken = new UserWithToken(user);
        //        userWithToken.AccessToken = GenerateAccessToken(user);

        //        return userWithToken;
        //    }

        //    return null;
        //}

        // GET: api/Users
        //[HttpPost("GetUserByAccessToken")]
        //public async Task<ActionResult<AppUser>> GetUserByAccessToken([FromBody] string accessToken)
        //{
        //    AppUser user = await GetUserFromAccessToken(accessToken);

        //    if (user != null)
        //    {
        //        return user;
        //    }

        //    return null;
        //}

        //private bool ValidateRefreshToken(AppUser user, string refreshToken)
        //{

        //    RefreshToken refreshTokenUser = _context.RefreshTokens.Where(rt => rt.Token == refreshToken)
        //                                        .OrderByDescending(rt => rt.ExpiryDate)
        //                                        .FirstOrDefault();

        //    if (refreshTokenUser != null && refreshTokenUser.UserId == user.Id
        //        && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
        //    {
        //        return true;
        //    }

        //    return false;
        //}

        //private async Task<AppUser> GetUserFromAccessToken(string accessToken)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var key = Encoding.ASCII.GetBytes(_configuration["Tokens:SecretKey"]);

        //        var tokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateIssuer = false,
        //            ValidateAudience = false
        //        };

        //        SecurityToken securityToken;
        //        var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

        //        JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

        //        if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
        //            return await _context.Users.Where(u => u.Id == Convert.ToInt32(userId)).FirstOrDefaultAsync();
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return new AppUser();
        //    }

        //    return new AppUser();
        //}

        //private RefreshToken GenerateRefreshToken()
        //{
        //    RefreshToken refreshToken = new RefreshToken();

        //    var randomNumber = new byte[32];
        //    using (var rng = RandomNumberGenerator.Create())
        //    {
        //        rng.GetBytes(randomNumber);
        //        refreshToken.Token = Convert.ToBase64String(randomNumber);
        //    }
        //    refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);

        //    return refreshToken;
        //}


        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("UpdateUser/{id}")]
        public async Task<ActionResult<AppUser>> PutUser(int id, AppUser user)
        {
            return await _userService.editUserInfo(id, user);
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("CreateUser")]
        public async Task<ActionResult<AppUser>> PostUser(AppUser user)
        {
            return await _userService.createUser(user);
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            return await _userService.deleteUser(id);
        }

        
    }
}
