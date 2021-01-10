using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
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
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult> AdminGetAll()
        {
            var exams = await _examService.GetAll();
            return Ok(exams);
        }
        [Authorize(Roles = "admin")]
        [HttpPost("Paged")]
        public async Task<ActionResult> AdminGetAllPaging(ExamPagingRequest model)
        {
            var exams = await _examService.GetAllPaging(model);
            return Ok(exams);
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult> AdminGetOne(int id)
        {
            var exam = await _examService.GetByID(id);
            return Ok(exam);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> AdminDeleteOne(int id)
        {
            var isDeleted = await _examService.Delete(id);
            return Ok(isDeleted);
        }
        //----------------------------------
        //================================== USER ===================================
        [HttpGet("{userID}")]
        public async Task<ActionResult> UserGetAll(int userID)
        {
            var exams = await _examService.GetAll(userID);
            return Ok(exams);
        }
        [HttpPost("Paged/{userID}")]
        public async Task<ActionResult> UserGetAllPaging(ExamPagingRequest model, int userID)
        {
            var exams = await _examService.GetAllPaging(model, userID);
            return Ok(exams);
        }
        [HttpGet("{id}/{userID}")]
        public async Task<ActionResult> UserGetOne(int id, int userID)
        {
            var exam = await _examService.GetByID(id, userID);
            return Ok(exam);
        }
        [HttpDelete("{id}/{userID}")]
        public async Task<ActionResult> UserDeleteOne(int id, int userID)
        {
            var isDeleted = await _examService.Delete(id, userID);
            return Ok(isDeleted);
        }
        //----------------------------------
        //================================== COMMON =====================================
        [HttpPost("{userID}")]
        public async Task<ActionResult> Create(Exam model, int userID)
        {
            var newExam = await _examService.Create(model, userID);
            return Ok(newExam);
        }
        [HttpPost("IncreaseAttemp/{id}")]
        public async Task<ActionResult> IncreaseAttemp(int id)
        {
            var newExam = await _examService.IncreaseAttemps(id);
            return Ok(newExam);
        }
        //----------------------------------
    }
}
