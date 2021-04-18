using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        //=============================== ADMIN ===================================


        // POST: api/Exams/Admin/Paged
        [Authorize(Roles = "admin")]
        [HttpPost("Admin/Paged")]
        public async Task<IActionResult> AdminGetAllPaging(ExamPagingRequest model, [FromQuery] string categoryID)
        {
            return Ok(await _examService.GetAllPaging(model));
        }

        // GET: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpGet("Admin/{id}")]
        public async Task<IActionResult> AdminGetOne(int id)
        {
            return Ok(await _examService.GetOne(id));
        }

        // DELETE: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpDelete("Admin/{id}")]
        public async Task<IActionResult> AdminDeleteOne(int id)
        {
            return Ok(await _examService.Delete(id));
        }

        // POST: api/Exams/Admin/DeleteMany
        [Authorize(Roles = "admin")]
        [HttpPost("Admin/DeleteMany")]
        public async Task<IActionResult> AdminDeleteMany(DeleteRangeModel<int> lstExamId)
        {
            return Ok(await _examService.DeleteMany(lstExamId));
        }

        // PUT: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpPut("Admin/{id}")]
        public async Task<IActionResult> AdminUpdateOne(int id, ExamModel model)
        {
            if(id != model.ID)
            {
                return BadRequest();
            }
            return Ok(await _examService.Update(model));
        }
        //----------------------------------
        //================================== USER ===================================
        [HttpGet("GetAll")]
        public async Task<IActionResult> UserGetOwned([FromQuery]int userID)
        {
            return Ok(await _examService.GetOwned(userID));
        }
        [HttpPost("Paged")]
        public async Task<IActionResult> UserGetAllPaging(ExamPagingRequest model,[FromQuery] int userID)
        {
            return Ok(await _examService.GetAllPaging(model, userID));
        }
        [HttpPost("Paged/Owned")]
        public async Task<IActionResult> UserGetOwnedPaging(ExamPagingRequest model, [FromQuery] int userID)
        {
            return Ok(await _examService.GetOwnedPaging(model, userID));
        }
        [HttpGet("User")]
        public async Task<IActionResult> UserGetOne([FromQuery]int id, [FromQuery]int userID)
        {
            return Ok(await _examService.GetOne(id, userID));
        }
        [HttpDelete("User")]
        public async Task<IActionResult> UserDeleteOne([FromQuery]int id, [FromQuery]int userID)
        {
            return Ok(await _examService.Delete(id, userID));
        }
        [HttpPut("User")]
        public async Task<IActionResult> UserUpdateOne([FromQuery]int id, [FromQuery]int userID, [FromBody]ExamModel model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }
            return Ok(await _examService.Update(model, userID));
        }
        //----------------------------------
        //================================== COMMON =====================================
        [HttpPost]
        public async Task<IActionResult> Create(ExamModel model, [FromQuery]int userID)
        {
            return Ok(await _examService.Create(model, userID));
        }
        [HttpPost("IncreaseAttemp/{id}")]
        public async Task<IActionResult> IncreaseAttemp(int id)
        {
            return Ok(await _examService.IncreaseAttemps(id));
        }
        //----------------------------------
    }
}
