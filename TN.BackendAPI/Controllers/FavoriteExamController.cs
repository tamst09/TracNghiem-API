using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Catalog.FavoriteExam;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("user")]
    public class FavoriteExamController : ControllerBase
    {
        private readonly IFavoriteExamService _examService;

        public FavoriteExamController(IFavoriteExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteExams()
        {
            var exams = await _examService.GetByUser(GetCurrentUserId());
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteExam([FromBody] AddFavoriteExamRequest favoriteExam)
        {
            var result = await _examService.Add(GetCurrentUserId(), favoriteExam.ExamId);
            if (result)
                return Ok(new ResponseBase{ success = true, msg = "Added." });
            else
                return Ok(new ResponseBase{ success = false, msg = "Some errors happened."});
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFavoriteExam([FromBody] AddFavoriteExamRequest favoriteExam)
        {
            var result = await _examService.Delete(GetCurrentUserId(), favoriteExam.ExamId);
            if (result)
                return Ok(new ResponseBase());
            else
                return Ok(new ResponseBase(success: false, msg: "Failed."));
        }

        private int GetCurrentUserId()
        {
            var tryParse = int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out int userId);
            return tryParse ? userId : -1;
        }
    }
}
