using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    public class FavoriteExamsController : Controller
    {
        private readonly IFavoriteExamService _favoriteExamService;

        public FavoriteExamsController(IFavoriteExamService favoriteExamService)
        {
            _favoriteExamService = favoriteExamService;
        }

        public async Task<IActionResult> GetAll()
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var result = await _favoriteExamService.GetExams(int.Parse(User.FindFirst("UserID").Value), token);
            return Json(new { success = result });
        }

        public async Task<IActionResult> Add([FromBody]int examId)
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var result = await _favoriteExamService.Add(int.Parse(User.FindFirst("UserID").Value), examId, token);
            return Json(new { success = result });
        }

        public async Task<IActionResult> Delete([FromBody]int examId)
        {
            var token = CookieEncoder.DecodeToken(Request.Cookies["access_token_cookie"]);
            var result = await _favoriteExamService.Delete(int.Parse(User.FindFirst("UserID").Value), examId, token);
            return Json(new { success = result });
        }
    }
}
