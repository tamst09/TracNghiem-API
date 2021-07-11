﻿using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;


namespace FrontEndWebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(IAccountService authClient, IWebHostEnvironment webHostEnvironment)
        {
            _accountService = authClient;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return RedirectToAction(nameof(Login));
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("access_token_cookie");
            HttpContext.Response.Cookies.Delete("refresh_token_cookie");
            HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            // send this to user email
            var resetCode = await _accountService.GetResetPasswordCode(model);
            if(resetCode != null)
            {
                ViewData["msg"] = "Check your email to complete changing your password";
                return View("/Views/Account/ForgotPasswordOnPost.cshtml");
            }
            ViewData["msg"] = "Your email is invalid. Please try again.";
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirm(ResetPasswordModel model)
        {
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> ForgotPasswordConfirmOnPost(ResetPasswordModel model)
        {
            var changePasswordResult = await _accountService.ResetPassword(model.ResetCode, model);
            if (changePasswordResult.msg==null)
            {
                ViewData["IsResetSuccessfully"] = true;
                return View();
            }
            else
            {
                ViewData["IsResetSuccessfully"] = false;
                return View();
            }
        }

        public IActionResult ChangePassword()
        {
            ViewData["success"] = false;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            ViewData["success"] = false;
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userID = User.FindFirst("UserID").Value;
            var accessToken = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var changeResult = await _accountService.ChangePassword(userID, model, accessToken);
            if (changeResult != null)
            {
                if (changeResult.msg != null)
                {
                    ViewData["msg"] = changeResult.msg;
                }
                ViewData["msg"] = "Đổi mật khẩu thành công";
                ViewData["success"] = true;
                HttpContext.Response.Cookies.Delete("access_token_cookie");
                HttpContext.Response.Cookies.Delete("refresh_token_cookie");
                await HttpContext.SignOutAsync();
                return View();
            }
            else
            {
                ViewData["msg"] = "Lỗi máy chủ. Vui lòng thử lại.";
                return View();
            }
        }

        [AllowAnonymous]
        public IActionResult Login(string username, string ReturnUrl)
        {

            ViewData["Title"] = "Log in";
            ViewData["ReturnUrl"] = ReturnUrl;
            return View(new LoginModel() { UserName = username });
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // jwt got from authentication API
            var result = await _accountService.Authenticate(model);

            if (!result.success)
            {
                ViewData["msg"] = result.msg;
                return View(model);
            }
            else
            {
                var userPrincipal = _accountService.ValidateToken(result.data.Access_Token);
                var authProperties = new AuthenticationProperties
                {
                    // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                    // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                    IsPersistent = model.Rememberme
                };

                await HttpContext.SignInAsync(userPrincipal, authProperties);
                
                if (model.Rememberme)
                {
                    HttpContext.Session.SetInt32("IsPersistent", 1);
                    HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(result.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(3), HttpOnly = true, Secure = true });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(result.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(7), HttpOnly = true, Secure = true });
                }
                else
                {
                    HttpContext.Session.SetInt32("IsPersistent", 0);
                    HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(result.data.Access_Token), new CookieOptions { HttpOnly = true, Secure = true });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(result.data.Refresh_Token), new CookieOptions { HttpOnly = true, Secure = true });
                }
                
                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.AvatarPhoto != null)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.AvatarPhoto.CopyToAsync(stream);
                }
                model.AvatarURL = UploadImageService.Instance().Upload(model.UserName, filePath);
                model.AvatarPhoto = null;
            }
            model.AvatarPhoto = null;
            //string url = model.AvatarPhotoURL;
            var user = await _accountService.Register(model);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.msg))
                {
                    ViewData["msg"] = user.msg;
                    return View(model);
                }
                ViewData["IsRegisterSuccess"] = true;
                return View(new RegisterModel());
            }
            ViewData["IsRegisterSuccess"] = false;
            return View(model);
        }

        [AllowAnonymous]
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
                    var jwttokenResponse = await _accountService.LoginFacebook(token);
                    if (jwttokenResponse != null)
                    {
                        var userPrincipal = _accountService.ValidateToken(jwttokenResponse.data.Access_Token);
                        var authProperties = new AuthenticationProperties
                        {
                            // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                            // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                            IsPersistent = true
                        };
                        await HttpContext.SignInAsync(userPrincipal, authProperties);
                        HttpContext.Session.SetInt32("IsPersistent", 1);
                        HttpContext.Response.Cookies.Append("access_token_cookie",CookieEncoder.EncodeToken(jwttokenResponse.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(4), HttpOnly = true, Secure = true });
                        HttpContext.Response.Cookies.Append("refresh_token_cookie",CookieEncoder.EncodeToken(jwttokenResponse.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(8), HttpOnly = true, Secure = true });
                        if (jwttokenResponse.data.isNewLogin)
                        {
                            int uid = Convert.ToInt32(userPrincipal.FindFirst("UserID").Value);
                            return RedirectToAction(nameof(UpdateProfile), new { id = uid });
                        }
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login));
                    }
                }
            }
            else if(provider == "Google")
            {
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    // get jwt from api
                    var jwttokenResponse = await _accountService.LoginGoogle(token);
                    if (jwttokenResponse != null && jwttokenResponse.data!=null)
                    {
                        var userPrincipal = _accountService.ValidateToken(jwttokenResponse.data.Access_Token);
                        var authProperties = new AuthenticationProperties
                        {
                            // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                            // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                            IsPersistent = true
                        };
                        await HttpContext.SignInAsync(userPrincipal, authProperties);
                        HttpContext.Session.SetInt32("IsPersistent", 1);
                        HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(4), HttpOnly = true, Secure = true });
                        HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(8), HttpOnly = true, Secure = true });
                        if (jwttokenResponse.data.isNewLogin)
                        {
                            int uid = Convert.ToInt32(userPrincipal.FindFirst("UserID").Value);
                            return RedirectToAction(nameof(UpdateProfile), new { id = uid });
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

        public async Task<IActionResult> ShowProfile()
        {
            var id = User.FindFirst("UserID");
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var user = await _accountService.GetUserInfo(Int32.Parse(id.Value), access_token);
            if (user.data != null)
                return View(user.data);
            return View();
        }

        public async Task<IActionResult> UpdateProfile(int id)
        {
            var userID = Int32.Parse(User.FindFirst("UserID").Value);
            
            if (id != userID)
            {
                // access denied
                return View("Views/Account/AccessDenied.cshtml");
            }
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var user = await _accountService.GetUserInfo(userID, access_token);
            if (user != null)
            {
                ViewData["uid"] = userID;
                return View(user.data);
            }
            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProfile(int id, UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(UpdateProfile), new
                {
                    id
                });
            }
            if (model.AvatarPhoto != null)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.AvatarPhoto.CopyToAsync(stream);
                }
                model.AvatarURL = UploadImageService.Instance().Upload(model.UserName, filePath);
                model.AvatarPhoto = null;
            }
            var userID = Int32.Parse(User.FindFirst("UserID").Value);
            if(id != userID)
            {
                return Redirect("/Views/Account/AccessDenied.cshtml");
            }
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var result = await _accountService.UpdateProfile(model, access_token);
            // success
            if (result!=null && result.data != null)
                return RedirectToAction(nameof(ShowProfile));
            // fail
            ViewData["msg"] = "Update failed";
            return RedirectToAction(nameof(UpdateProfile), new
            {
                id = id
            });
        }
    }
}
