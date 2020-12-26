using FrontEndWebApp.Services;
using FrontEndWebApp.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.User;

namespace FrontEndWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="admin")]
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthClient _authClient;
        private HttpClient httpClient;

        public UsersController(IHttpClientFactory httpClientFactory, IAuthClient authClient)
        {
            _httpClientFactory = httpClientFactory;
            httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(ConstStrings.BaseUrl);
            _authClient = authClient;
        }

        // GET: UsersController
        public async Task<ActionResult> Index()
        {
            List<UserViewModel> lstAllUser = new List<UserViewModel>();
            try
            {
                var token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                if (token != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await httpClient.GetAsync("/api/users/");
                if (response.IsSuccessStatusCode)
                {
                    var lstuser = await response.Content.ReadAsStringAsync();
                    lstAllUser = JsonConvert.DeserializeObject<List<UserViewModel>>(lstuser);
                    return View(lstAllUser);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) 
                {
                    // if not authorized
                    return View(new List<UserViewModel>());
                }
                else
                {
                    return View(new List<UserViewModel>());
                }
            }
            catch (Exception)
            {
                //throw;
                return View(new List<UserViewModel>());
            }
        }

        // GET: UsersController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var access_token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);

                var user = await _authClient.GetUserInfo(id, access_token);

                return View(user);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Users");
            }
        }

        // GET: UsersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsersController/Create
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
                var token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                if (token != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var json = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/api/users/CreateUser/",httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    JwtResponse jwtResponse = JsonConvert.DeserializeObject<JwtResponse>(result);
                    if(jwtResponse.Error!=null || jwtResponse.Access_Token == null)
                    {
                        ViewData["Error"] = jwtResponse.Error;
                        return View(model);
                    }
                    return RedirectToAction(nameof(Index));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // if not authorized
                    return View(model);
                }
                else
                {
                    return View(model);
                }
            }
            catch
            {
                ViewData["Error"] = "Can't connect to server. Please do it again.";
                return View();
            }
        }

        // GET: UsersController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var access_token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                var user = await _authClient.GetUserInfo(id, access_token);
                if(user!=null)
                    return View(user);
                else
                    return RedirectToAction("Index", "Users");
            }
            catch
            {
                return RedirectToAction("Index", "Users");
            }
        }

        // POST: UsersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UserViewModel model)
        {
            try
            {
                var access_token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                var userUpdated = await _authClient.UpdateProfile(id, model, access_token);
                return RedirectToAction("Details", "Users", new { id = userUpdated.Id });
            }
            catch
            {
                return View();
            }
        }

        // GET: UsersController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UsersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
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
                var token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                if (token != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await httpClient.PostAsync("/api/users/LockUser/"+id.ToString(), null);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
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

        [HttpPost]
        public async Task<ActionResult> RestoreUser(int id)
        {
            try
            {
                var token = TokenUtils.DecodeToken(Request.Cookies["access_token_cookie"]);
                if (token != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var response = await httpClient.PostAsync("/api/users/RestoreUser/" + id.ToString(), null);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
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
