using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class UsersController : Controller
    {
        private readonly IAccountService _accountService;   // thao tac voi tai khoan
        private readonly IUserManage _userManage;           // quan ly user cua admin

        public UsersController(IAccountService accountService, IUserManage userManage)
        {
            _accountService = accountService;
            _userManage = userManage;
        }

        // GET: Users
        public async Task<ActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                //List<UserViewModel> lstAllUser = new List<UserViewModel>();
                var model = new UserPagingRequest()
                {
                    keyword = keyword,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var listUserResult = await _userManage.GetListUserPaged(model, token);
                return View(listUserResult.data);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int id)
        {
            
            try
            {
                var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var user = await _accountService.GetUserInfo(id, access_token);
                return View(user.data);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Users");
            }
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var createUserResult = await _userManage.CreateUser(model, token);
                //error
                if(createUserResult.msg!=null || createUserResult.data == null)
                {
                    ViewData["Error"] = createUserResult.msg;
                    return View(model);
                }
                //success
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["Error"] = "Can't connect to server. Please try later.";
                return View();
            }
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
           
            try
            {
                var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var user = await _accountService.GetUserInfo(id, access_token);
                if(user.data!=null)
                    return View(user.data);
                else
                    return RedirectToAction("Index", "Users");
            }
            catch
            {
                return RedirectToAction("Index", "Users");
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UserViewModel model)
        {
            try
            {
                var access_token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var userUpdated = await _accountService.UpdateProfile(id, model, access_token);
                return RedirectToAction("Details", "Users", new { id = userUpdated.data.Id });
            }
            catch
            {
                return View();
            }
        }
        // POST: Users/LockUser/5
        [HttpPost]
        public async Task<ActionResult> LockUser(int id)
        {
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var lockUserResult = await _userManage.LockUser(id, token);
                if (lockUserResult.msg == null)
                {
                    return Json(new
                    {
                        statusChanged = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        statusChanged = false
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    statusChanged = false
                });
            }
        }
        // POST: Users/RestoreUser/5
        [HttpPost]
        public async Task<ActionResult> RestoreUser(int id)
        {
            try
            {
                var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
                var lockUserResult = await _userManage.RestoreUser(id, token);
                if (lockUserResult.msg == null)
                {
                    return Json(new
                    {
                        statusChanged = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        statusChanged = false
                    });
                }
            }
            catch
            {
                return Json(new
                {
                    statusChanged = false
                });
            }
        }
    }
}
