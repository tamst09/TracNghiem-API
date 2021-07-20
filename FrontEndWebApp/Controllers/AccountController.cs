using FrontEndWebApp.Services;
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
            //HttpContext.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            // send this to user email
            var resetCode = await _accountService.GetResetPasswordCode(model);
            if (resetCode.success && resetCode.data != null)
            {
                ViewData["msg"] = "Check your email to complete changing your password";
                return View("/Views/Account/ForgotPasswordOnPost.cshtml");
            }
            ViewData["msg"] = "Your email is invalid. Please try again.";
            return View(model);
        }

        public ActionResult ForgotPasswordConfirm(ResetPasswordModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPasswordConfirmOnPost(ResetPasswordModel model)
        {
            var changePasswordResult = await _accountService.ResetPassword(model);
            ViewData["IsResetSuccessfully"] = changePasswordResult.msg == null ? true : false;
            return View();
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
            var getProfileRes = await _accountService.GetUserInfoByToken();
            if (!getProfileRes.success)
            {
                ViewData["msg"] = "Cannot get user info. Login again.";
                return View();
            }
            var changeResult = await _accountService.ChangePassword(getProfileRes.data.UserId, model);
            ViewData["success"] = changeResult.success;
            ViewData["msg"] = changeResult.msg;
            if (changeResult.success)
            {
                HttpContext.Response.Cookies.Delete("access_token_cookie");
                HttpContext.Response.Cookies.Delete("refresh_token_cookie");
            }
            return View();
        }

        public IActionResult Login(string username, string ReturnUrl)
        {

            ViewData["Title"] = "Log in";
            ViewData["ReturnUrl"] = ReturnUrl;
            return View(new LoginModel() { UserName = username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // jwt got from authentication API
            var response = await _accountService.Authenticate(model);

            if (!response.success)
            {
                ViewData["msg"] = response.msg;
                return View(model);
            }
            else
            {
                //var userPrincipal = _accountService.ValidateToken(response.data.Access_Token);
                var authProperties = new AuthenticationProperties
                {
                    // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                    // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                    IsPersistent = model.Rememberme
                };

                //await HttpContext.SignInAsync(userPrincipal, authProperties);

                if (model.Rememberme)
                {
                    HttpContext.Response.Cookies.Append("access_token_cookie",
                        CookieEncoder.EncodeToken(response.data.Access_Token),
                        new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(3),
                            HttpOnly = true,
                            Secure = true
                        });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie",
                        CookieEncoder.EncodeToken(response.data.Refresh_Token),
                        new CookieOptions
                        {
                            Expires = DateTime.UtcNow.AddDays(7),
                            HttpOnly = true,
                            Secure = true
                        });
                }
                else
                {
                    HttpContext.Response.Cookies.Append("access_token_cookie",
                        CookieEncoder.EncodeToken(response.data.Access_Token),
                        new CookieOptions { HttpOnly = true, Secure = true });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie",
                        CookieEncoder.EncodeToken(response.data.Refresh_Token),
                        new CookieOptions { HttpOnly = true, Secure = true });
                }

                if (!string.IsNullOrEmpty(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                return RedirectToAction("Index", "Home");
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
            if (model.AvatarPhoto != null)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.AvatarPhoto.CopyToAsync(stream);
                }
                model.AvatarURL = UploadImageService.Instance().Upload(model.UserName, filePath);
            }
            model.AvatarPhoto = null;
            var user = await _accountService.Register(model);
            if (user.success)
            {
                if (!string.IsNullOrEmpty(user.msg))
                {
                    ViewData["msg"] = user.msg;
                }
            }
            ViewData["IsRegisterSuccess"] = user.success;
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
                        //var userPrincipal = _accountService.ValidateToken(jwttokenResponse.data.Access_Token);
                        //var authProperties = new AuthenticationProperties
                        //{
                        //    IsPersistent = true
                        //};
                        //await HttpContext.SignInAsync(userPrincipal, authProperties);
                        //HttpContext.Session.SetInt32("IsPersistent", 1);
                        HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(3), HttpOnly = true, Secure = true });
                        HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(7), HttpOnly = true, Secure = true });
                        if (jwttokenResponse.data.isNewLogin)
                        {
                            //int uid = Convert.ToInt32(userPrincipal.FindFirst("UserID").Value);
                            //return RedirectToAction(nameof(UpdateProfile), new { id = uid });
                        }
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Login));
                    }
                }
            }
            else if (provider == "Google")
            {
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    // get jwt from api
                    var jwttokenResponse = await _accountService.LoginGoogle(token);
                    if (jwttokenResponse != null && jwttokenResponse.data != null)
                    {
                        //var userPrincipal = _accountService.ValidateToken(jwttokenResponse.data.Access_Token);
                        //var authProperties = new AuthenticationProperties
                        //{
                        // set false -> tạo ra cookie phiên -> thoát trình duyệt cookie bị xoá
                        // set true -> cookie có thời hạn đc set trong Startup.cs và ko bị mất khi thoát
                        //IsPersistent = true
                        //};
                        //await HttpContext.SignInAsync(userPrincipal, authProperties);
                        // HttpContext.Session.SetInt32("IsPersistent", 1);
                        HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(4), HttpOnly = true, Secure = true });
                        HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(jwttokenResponse.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(8), HttpOnly = true, Secure = true });
                        if (jwttokenResponse.data.isNewLogin)
                        {
                            //int uid = Convert.ToInt32(userPrincipal.FindFirst("UserID").Value);
                            //return RedirectToAction(nameof(UpdateProfile), new { id = uid });
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
            var getProfile = await _accountService.GetUserInfoByToken();
            if (!getProfile.success)
            {
                return View();
            }
            var user = await _accountService.GetUserInfo(getProfile.data.UserId);
            if (user.success)
                return View(user.data);
            return View();
        }

        public async Task<IActionResult> UpdateProfile()
        {
            var getProfile = await _accountService.GetUserInfoByToken();
            ViewData["msg"] = "";
            if (!getProfile.success)
            {
                ViewData["msg"] = getProfile.msg;
                return View(new UserViewModel());
            }
            var getUserResponse = await _accountService.GetUserInfo(getProfile.data.UserId);
            if (getUserResponse.success)
            {
                ViewData["msg"] = getUserResponse.msg;
                ViewData["uid"] = getProfile.data.UserId;
                return View(getUserResponse.data);
            }

            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostUpdateProfile(UserViewModel model)
        {
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
            var updateResponse = await _accountService.UpdateProfile(model);
            // success
            if (updateResponse.success)
                return RedirectToAction(nameof(ShowProfile));
            // fail
            ViewData["msg"] = updateResponse.msg;
            return RedirectToAction(nameof(UpdateProfile), new { id = model.Id });
        }
    }
}
