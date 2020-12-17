using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.FacebookAuth;

namespace FrontEndWebApp.Controllers
{
    public class AuthController : Controller
    {
        private IUserClient _userClient;


        public AuthController(IUserClient userClient)
        {
            _userClient = userClient;
        }

        //login
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            ViewData["Title"] = "Log in";
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token_cookie");
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            // jwt got from authentication API 
            var token = await _userClient.Authenticate(request);
            
            if(token == "wrong")
            {
                ViewData["msg"] = "Invalid username or password";
                return View(request);
            }
            else if(token == "notfound")
            {
                ViewData["msg"] = "User is not found";
                return View(request);
            }
            else if(token=="error")
            {
                ViewData["msg"] = "Error";
                return View(request);
                //return RedirectToAction("Error");
            }
            else
            {
                HttpContext.Response.Cookies.Append("access_token_cookie", token, new CookieOptions { HttpOnly = true, Secure = true });
                var userPrincipal = _userClient.ValidateToken(token);
                var authProperties = new AuthenticationProperties
                {
                    // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                    // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                    IsPersistent = false
                };
                await HttpContext.SignInAsync(userPrincipal,authProperties);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoginFacebook([FromBody] Root root)
        {
            if (root == null)
            {
                return RedirectToAction("Login", "Auth");
            }
            //Root root = JsonConvert.DeserializeObject<Root>(response);
            /*CreateFacebookUserResult loginfbUser = await _userClient.LoginFacebook(root.authResponse.accessToken);
            if(loginfbUser != null)
            {
                if (loginfbUser.isNewUser)
                {
                    RegisterModel user = new RegisterModel()
                    {
                        FirstName = loginfbUser.User.FirstName,
                        LastName = loginfbUser.User.LastName,
                        Email = loginfbUser.User.Email,
                        UserName = loginfbUser.User.Email
                    };
                    //return RedirectToAction("UpdateProfile","Auth",user);
                    return Ok("updateprofile");
                }
                //return RedirectToAction("Index", "Home");
                return Ok("returntoindex");
            }*/
            //return RedirectToAction("Index", "Home");
            return Json(new { redirectUrl = Url.Action("Index", "Home") });
        }

        public async Task<IActionResult> ShowProfile(int id)
        {
            var user = await _userClient.GetUserInfo(id);
            if(user!=null)
                return View(user);
            else return RedirectToAction("Index", "Home");
        }


        public IActionResult UpdateProfile(RegisterModel user)
        {
            if (user != null)
            {
                return View(user);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(int id, RegisterModel user)
        {
            var result = await _userClient.UpdateProfile(id, user);
            return RedirectToAction(nameof(ShowProfile));
        }
    }
}
