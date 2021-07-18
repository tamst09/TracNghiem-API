using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.ViewModels.Catalog.FavoriteExam;

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
            var getAllRequest = new GetAllFavoriteRequest()
            {
                userId = 1
            };
            _favoriteExamService.GetExams(getAllRequest);
        }

        public async Task<IActionResult> GetAll()
        {
            var getAllRequest = new GetAllFavoriteRequest() { userId = int.Parse(User.FindFirst("UserID").Value) };
            var result = await _favoriteExamService.GetExams(getAllRequest);
            return Ok(result);
        }

        public async Task<IActionResult> Add([FromBody]int examId)
        {
            var addFavoriteRequest = new AddFavoriteExamRequest()
            {
                examId = examId,
                userId = int.Parse(User.FindFirst("UserID").Value)
            };
            var result = await _favoriteExamService.Add(addFavoriteRequest);
            return Json(result);
        }

        public async Task<IActionResult> Delete([FromBody]int examId)
        {
            var deleteFavoriteRequest = new DeleteFavoriteExamRequest()
            {
                examId = examId,
                userId = int.Parse(User.FindFirst("UserID").Value)
            };
            var result = await _favoriteExamService.Delete(deleteFavoriteRequest);
            return Json(result);
        }
    }
}
