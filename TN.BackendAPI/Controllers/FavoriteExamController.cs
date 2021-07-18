using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
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
        public async Task<IActionResult> GetFavoriteExams(int userId)
        {
            var exams = await _examService.GetByUser(userId);
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteExam([FromBody] FavoriteExamVM favoriteExam)
        {
            var result = await _examService.Add(favoriteExam.UserId, favoriteExam.ExamId);
            if (result)
                return Ok(new ResponseBase<bool> { success = true, msg = "Added", data = result });
            else
                return Ok(new ResponseBase<bool> { success = false, msg = "Some errors happened", data = result });
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFavoriteExam([FromBody] FavoriteExamVM favoriteExam)
        {
            var result = await _examService.Delete(favoriteExam.UserId, favoriteExam.ExamId);
            if (result)
                return Ok(new ResponseBase<bool> { success = true, msg = "Deleted", data = result });
            else
                return Ok(new ResponseBase<bool> { success = false, msg = "Some errors happened", data = result });
        }
    }
}
