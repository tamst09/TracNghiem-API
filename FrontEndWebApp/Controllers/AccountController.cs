using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
        // ========================== COMMON ==========================
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
        public ActionResult ForgotPasswordConfirm(ResetPasswordModel model)
        {
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> ForgotPasswordConfirmOnPost(ResetPasswordModel model)
        {
            var changePasswordResult = await _accountService.ChangePassword(model.ResetCode, model);
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
        //====================================================

        //========================== NORMAL LOGIN ==========================
        public IActionResult Login(string username, string ReturnUrl)
        {

            ViewData["Title"] = "Log in";
            ViewData["ReturnUrl"] = ReturnUrl;
            return View(new LoginModel() { UserName = username });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // jwt got from authentication API
            var result = await _accountService.Authenticate(model);

            if (!string.IsNullOrEmpty(result.msg))
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
                HttpContext.SignInAsync(userPrincipal, authProperties);
                
                if (model.Rememberme)
                {
                    HttpContext.Session.SetInt32("IsPersistent", 1);
                    HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(result.data.Access_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(4), HttpOnly = true, Secure = true });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(result.data.Refresh_Token), new CookieOptions { Expires = DateTime.UtcNow.AddDays(8), HttpOnly = true, Secure = true });
                }
                else
                {
                    HttpContext.Session.SetInt32("IsPersistent", 0);
                    HttpContext.Response.Cookies.Append("access_token_cookie", CookieEncoder.EncodeToken(result.data.Access_Token), new CookieOptions { HttpOnly = true, Secure = true });
                    HttpContext.Response.Cookies.Append("refresh_token_cookie", CookieEncoder.EncodeToken(result.data.Refresh_Token), new CookieOptions { HttpOnly = true, Secure = true });
                }
                
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
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
            if (model.AvatarPhotoURL != null)
            {
                string folder = "images/cover/user/";
                folder += Guid.NewGuid().ToString() + "_" + model.AvatarFile.FileName;
                model.AvatarPhotoURL = "/" + folder;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                await model.AvatarFile.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
            model.AvatarFile = null;
            string url = model.AvatarPhotoURL;
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

        //====================================================

        //========================== EXTERNAL LOGIN ==========================
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
            else
            {
                return RedirectToAction(nameof(Login));
            }
        }
        //====================================================

        //========================== PROFILE ==========================
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
                string folder = "images/cover/user/";
                folder += Guid.NewGuid().ToString() + "_" + model.AvatarPhoto.FileName;
                model.Avatar = "/"+folder;
                string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
                await model.AvatarPhoto.CopyToAsync(new FileStream(serverFolder, FileMode.Create));
            }
            model.AvatarPhoto = null;
            string url = model.Avatar;
            var userID = Int32.Parse(User.FindFirst("UserID").Value);
            if(id != userID)
            {
                return Redirect("/Views/Account/AccessDenied.cshtml");
            }
            var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var result = await _accountService.UpdateProfile(id, model, access_token);
            // success
            if (result.data != null)
                return RedirectToAction(nameof(ShowProfile), new { id = result.data.Id });
            // fail
            return RedirectToAction(nameof(UpdateProfile), new
            {
                id
            });
        }
        //public async Task<IActionResult> AddPassword(int id)
        //{
        //    var user = await _userClient.GetUserInfo(id);
        //    if (user != null)
        //    {
        //        ViewData["email"] = user.Email;
        //        return View(new ResetPasswordModel() { Email = user.Email });
        //    }
        //    return View();
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddPassword(ResetPasswordModel request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(request);
        //    }
        //    var result = await _userClient.AddPassword(request);
        //    if (result != null)
        //        return RedirectToAction(nameof(UpdateProfile), new { id = result.Id });
        //    return View(request);
        //}
        //==============================================================================





    }
}
