using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;

namespace FrontEndWebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserClient _userClient;

        public AuthController(IUserClient userClient)
        {
            _userClient = userClient;
        }
        // ----------------------------------COMMON---------------------------------------
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }


        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token_cookie");
            HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public async Task<IActionResult> ShowProfile()
        {
            var id = User.FindFirst("UserID");
            var user = await _userClient.GetUserInfo(Int32.Parse(id.Value));
            if (user != null)
                return View(user);
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
        //-----------------------------------------------------------------------------------

        //------------------------------------NORMAL LOGIN-----------------------------------
        public IActionResult Login(string username)
        {
            ViewData["Title"] = "Log in";
            return View(new LoginModel() { UserName = username });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            
            // jwt got from authentication API 
            var token = _userClient.Authenticate(request).Result;

            if (token == "wrong")
            {
                ViewData["msg"] = "Invalid username or password";
                return View(request);
            }
            else if (token == "notfound")
            {
                ViewData["msg"] = "User is not found";
                return View(request);
            }
            else if (token == "error")
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
                
                HttpContext.SignInAsync(userPrincipal, authProperties);


                return RedirectToAction("Index", "Home");
                //if (HttpContext.User.IsInRole(""))
                //{
                //    // direct to admin page
                //    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                //}
                //else
                //{
                //    // direct to public page
                //    return RedirectToAction("Index", "Home", new { Area = "User" });
                //}
                
                //return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userClient.Register(model);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.Error))
                {
                    ViewData["msg"] = user.Error;
                    return View(model);
                }
                return RedirectToAction(nameof(Login));
            }
            ViewData["msg"] = "Unsuccessful register.";
            return View(model);
        }

        //----------------------------------------------------------------------------------------------------

        //---------------------------------------External Login-----------------------------------------------
        [HttpPost]
        public async Task<IActionResult> ExternalLogin(string provider, string token)
        {
            if (provider == "Facebook")
            {
                if (string.IsNullOrEmpty(token))
                {
                    // direct to facebook login page
                    // to get fb access token
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    // get jwt from api
                    var jwttoken = await _userClient.LoginFacebook(token);
                    if (jwttoken != null)
                    {
                        var userPrincipal = _userClient.ValidateToken(jwttoken.Access_Token);
                        var authProperties = new AuthenticationProperties
                        {
                            // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                            // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                            IsPersistent = false
                        };
                        await HttpContext.SignInAsync(userPrincipal, authProperties);
                        HttpContext.Response.Cookies.Append("access_token_cookie", jwttoken.Access_Token, new CookieOptions { HttpOnly = true, Secure = true });
                        if (jwttoken.isNewLogin)
                        {
                            int uid = Convert.ToInt32(userPrincipal.FindFirst("UserID").Value);
                            return RedirectToAction(nameof(AddPassword), new { id = uid });
                        }
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login));
                    }
                }
            }
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }
        //----------------------------------------------------------------------------------------------

        //-----------------------------------------PROFILE----------------------------------------------
        
        public async Task<IActionResult> UpdateProfile(int id)
        {
            var user = await _userClient.GetUserInfo(id);
            if (user != null)
            {
                ViewData["uid"] = id;
                return View(user);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProfile(int id, UserViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UpdateProfile), new
                {
                    id
                });
            }
            request.Id = id;
            var result = await _userClient.UpdateProfile(id, request);
            // success
            if (result != null)
                return RedirectToAction(nameof(ShowProfile), new { id = result.Id });
            // fail
            return RedirectToAction(nameof(UpdateProfile), new
            {
                id
            });
        }
        public async Task<IActionResult> AddPassword(int id)
        {
            var user = await _userClient.GetUserInfo(id);
            if (user != null)
            {
                ViewData["email"] = user.Email;
                return View(new ResetPasswordModel() { Email = user.Email });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPassword(ResetPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }
            var result = await _userClient.AddPassword(request);
            if (result != null)
                return RedirectToAction(nameof(UpdateProfile), new { id = result.Id });
            return View(request);
        }
        //------------------------------------------------------------------------------------------


        

    }
}
