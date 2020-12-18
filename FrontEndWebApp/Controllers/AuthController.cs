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
            return RedirectToAction(nameof(HomeController.Index), "Home");
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
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public async Task<IActionResult> ShowProfile(int id)
        {
            var user = await _userClient.GetUserInfo(id);
            if(user!=null)
                return View(user);
            return View();
        }


        public async Task<IActionResult> UpdateProfile(int id)
        {
            var u = await _userClient.GetUserInfo(id);
            RegisterModel user = new RegisterModel();
            user.Id = id;
            user.FirstName = u.FirstName;
            user.LastName = u.LastName;
            user.Email = u.Email;
            user.DoB = u.DoB;
            user.PhoneNumber = u.PhoneNumber;
            user.UserName = u.UserName;
            if (user != null)
            {
                ViewData["uid"] = id;
                return View(user);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProfile(int id, RegisterModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UpdateProfile), new
                {
                    id = id
                    //UserName = request.UserName,
                    //FirstName = request.FirstName,
                    //LastName = request.LastName,
                    //Email = request.Email,
                    //PhoneNumber = request.PhoneNumber,
                    //Password = request.Password,
                    //DoB = request.DoB,
                    //ConfirmPassword = request.ConfirmPassword
                });
            }
            request.Id = id;
            var result = await _userClient.UpdateProfile(id, request);
            return RedirectToAction(nameof(ShowProfile),id);
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string token)
        {
            if (provider == "Facebook")
            {
                if(string.IsNullOrEmpty(token))
                {
                    // direct to facebook login page
                    // to get fb access token
                    return RedirectToAction(nameof(FBLogin));
                }
                else
                {
                    // handle with token
                    // create or get user with token
                    CreateFacebookUserResult createFacebookUserResult = await _userClient.LoginFacebook(token);
                    if(createFacebookUserResult != null)
                    {
                        // user not exists
                        if(createFacebookUserResult.isNewUser)
                        {
                            // update profile
                            return RedirectToAction(nameof(UpdateProfile), new { 
                                id = createFacebookUserResult.User.Id,
                                UserName = createFacebookUserResult.User.UserName,
                                FirstName = createFacebookUserResult.User.FirstName,
                                LastName = createFacebookUserResult.User.LastName,
                                Email = createFacebookUserResult.User.Email
                            });
                        }
                        // user exists
                        // enter password to  continue
                        LoginModel request = new LoginModel()
                        {
                            UserName = createFacebookUserResult.User.UserName
                        };
                        return RedirectToAction(nameof(OnGetConfirmLogin), request);
                    }
                    // create failed
                    else
                    {
                        return RedirectToAction("Login");
                    }
                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // login with only password
        public IActionResult OnGetConfirmLogin(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmLogin(LoginModel model)
        {
            // jwt got from authentication API
            var jwttoken = await _userClient.Authenticate(model);

            if (jwttoken == "wrong")
            {
                ViewData["msg"] = "Invalid username or password";
                return View(model);
            }
            else if (jwttoken == "notfound")
            {
                ViewData["msg"] = "User is not found";
                return View(model);
            }
            else if (jwttoken == "error")
            {
                ViewData["msg"] = "Error";
                return View(model);
                //return RedirectToAction("Error");
            }
            else
            {
                HttpContext.Response.Cookies.Append("access_token_cookie", jwttoken, new CookieOptions { HttpOnly = true, Secure = true });
                var userPrincipal = _userClient.ValidateToken(jwttoken);
                var authProperties = new AuthenticationProperties
                {
                    // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                    // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                    IsPersistent = false
                };
                await HttpContext.SignInAsync(userPrincipal, authProperties);
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        // get facebook access token and save to session storage
        public IActionResult FBLogin()
        {
            return View();
        }
    }
}
