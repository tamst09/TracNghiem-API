using FrontEndWebApp.Areas.Admin.AdminServices;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.User;
using TN.ViewModels.Common;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly IAccountService _accountService;   // thao tac voi tai khoan
        private readonly IUserManage _userManage;           // quan ly user cua admin
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(IAccountService accountService, IUserManage userManage, IWebHostEnvironment webHostEnvironment)
        {
            _accountService = accountService;
            _userManage = userManage;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<ActionResult> Index(string keyword, int pageIndex = 1, int pageSize = 10)
        {
            var model = new UserPagingRequest()
            {
                keyword = keyword,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var listUserResult = await _userManage.GetListUserPaged(model);
            return View(listUserResult.data);
        }

        // GET: Users/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await _accountService.GetUserInfo(id);
            return View(user.data);
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
            var createUserResult = await _userManage.CreateUser(model);
            //error
            if (!createUserResult.success)
            {
                ViewData["Error"] = createUserResult.msg;
                return View(model);
            }
            //success
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var user = await _accountService.GetUserInfo(id);
            if (user.success)
                return View(user.data);
            else
                return RedirectToAction("Index", "Users");
        }

        // POST: Users/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
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
            var userUpdated = await _accountService.UpdateProfile(model);
            ViewData["msg"] = userUpdated.msg;
            if (userUpdated.success)
            {
                return RedirectToAction("Details", "Users", new { id = model.Id });
            }
            return View(model);
        }
        // POST: Users/LockUser/5
        [HttpPost]
        public async Task<ActionResult> LockUser(int id)
        {
            var lockUser = await _userManage.LockUser(id);
            return Json(new
            {
                statusChanged = lockUser.success
            });
        }
        // POST: Users/RestoreUser/5
        [HttpPost]
        public async Task<ActionResult> RestoreUser(int id)
        {
            var restoreUser = await _userManage.RestoreUser(id);
            return Json(new
            {
                statusChanged = restoreUser.success
            });
        }
    }
}
