using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using TN.Data.DataContext;
using TN.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TN.ViewModels.Catalog.Users;
using TN.ViewModels.Catalog;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TNDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly JWTSettings _jwtsettings;
        public UsersController(TNDbContext context, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IOptions<JWTSettings> jwtsettings)
        {
            _context = context;
            _jwtsettings = jwtsettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: api/Users
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("GetUser/{id}")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // GET: api/Users/5
        [HttpGet("GetUserDetails/{id}")]
        public async Task<ActionResult<AppUser>> GetUserDetails(int id)
        {
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // POST: api/Users
        [HttpPost("login")]
        public async Task<ActionResult<UserWithToken>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return NotFound("User is Not found");
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!result.Succeeded)
            {
                return BadRequest("Wrong username or password");
            }

            UserWithToken userWithToken = null;
            RefreshToken refreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            userWithToken = new UserWithToken(user);
            userWithToken.RefreshToken = refreshToken.Token;

            if (userWithToken == null)
            {
                return NotFound();
            }

            //sign your token here here..
            userWithToken.AccessToken = GenerateAccessToken(user);
            return Ok(userWithToken);
        }

        // POST: api/Users
        [HttpPost("register")]
        public async Task<ActionResult<UserWithToken>> RegisterUser([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser()
            {
                UserName = request.UserName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                DoB = request.DoB

            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                UserWithToken userWithToken = null;
                RefreshToken refreshToken = GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _context.SaveChangesAsync();

                userWithToken = new UserWithToken(user);
                userWithToken.RefreshToken = refreshToken.Token;
                if (userWithToken == null)
                {
                    return NotFound();
                }
                //sign your token here here..
                userWithToken.AccessToken = GenerateAccessToken(user);
                return Ok(userWithToken);
            }
            return BadRequest("Register unsuccessfully");


        }

        // GET: api/Users
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            AppUser user = await GetUserFromAccessToken(refreshRequest.AccessToken);

            if (user != null && ValidateRefreshToken(user, refreshRequest.RefreshToken))
            {
                UserWithToken userWithToken = new UserWithToken(user);
                userWithToken.AccessToken = GenerateAccessToken(user);

                return userWithToken;
            }

            return null;
        }

        // GET: api/Users
        [HttpPost("GetUserByAccessToken")]
        public async Task<ActionResult<AppUser>> GetUserByAccessToken([FromBody] string accessToken)
        {
            AppUser user = await GetUserFromAccessToken(accessToken);

            if (user != null)
            {
                return user;
            }

            return null;
        }

        private bool ValidateRefreshToken(AppUser user, string refreshToken)
        {

            RefreshToken refreshTokenUser = _context.RefreshTokens.Where(rt => rt.Token == refreshToken)
                                                .OrderByDescending(rt => rt.ExpiryDate)
                                                .FirstOrDefault();

            if (refreshTokenUser != null && refreshTokenUser.UserId == user.Id
                && refreshTokenUser.ExpiryDate > DateTime.UtcNow)
            {
                return true;
            }

            return false;
        }

        private async Task<AppUser> GetUserFromAccessToken(string accessToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken securityToken;
                var principle = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);

                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userId = principle.FindFirst(ClaimTypes.Name)?.Value;
                    return await _context.Users.Where(u => u.Id == Convert.ToInt32(userId)).FirstOrDefaultAsync();
                }
            }
            catch (Exception)
            {
                return new AppUser();
            }

            return new AppUser();
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
            refreshToken.ExpiryDate = DateTime.UtcNow.AddDays(7);

            return refreshToken;
        }

        private string GenerateAccessToken(AppUser user)
        {
            var roles = _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtsettings.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Role, string.Join(";",roles))
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("UpdateUser/{id}")]
        public async Task<IActionResult> PutUser(int id, AppUser user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("CreateUser")]
        public async Task<ActionResult<AppUser>> PostUser(AppUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/Users/5
        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult<AppUser>> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
