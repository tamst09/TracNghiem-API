using FrontEndWebApp.Areas.User.Services;
using FrontEndWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TN.Data.Entities;
using TN.ViewModels.Catalog.FavoriteExam;

namespace FrontEndWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class FavoriteExamsController : Controller
    {
        private readonly IApiHelper _apiHelper;

        public FavoriteExamsController(IApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public async Task<IActionResult> GetAll()
        {
            var result = await _apiHelper.NonBodyQueryAsync<List<Exam>>(HttpMethod.Get, "/api/FavoriteExam");
            return Ok(result);
        }

        public async Task<IActionResult> Add([FromQuery]int examId)
        {
            var addFavoriteRequest = new AddFavoriteExamRequest()
            {
                ExamId = examId,
            };
            var result = await _apiHelper.CommandAsync(HttpMethod.Post,"/api/FavoriteExam", addFavoriteRequest);
            return Json(result);
        }

        public async Task<IActionResult> Delete([FromQuery]int examId)
        {
            var deleteFavoriteRequest = new DeleteFavoriteExamRequest()
            {
                examId = examId,
            };
            var result= await _apiHelper.CommandAsync(HttpMethod.Post, "/api/FavoriteExam/Remove", deleteFavoriteRequest);
            return Json(result);
        }
    }
}
