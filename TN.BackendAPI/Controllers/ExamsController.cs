using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Exam;
using TN.ViewModels.Catalog.Exams;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamsController : ControllerBase
    {
        private readonly IExamAdminService _examAdminService;
        private readonly IExamUserService _examUserService;

        public ExamsController(IExamAdminService examAdminService, IExamUserService examUserService)
        {
            _examAdminService = examAdminService;
            _examUserService = examUserService;
        }

        #region Admin
        [Authorize(Roles = "admin")]
        [HttpGet("Admin")]
        public async Task<IActionResult> AdminGetExams()
        {
            var exams = await _examAdminService.GetAll();
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Admin/Paged")]
        public async Task<IActionResult> AdminGetExamsPaged(ExamPagingRequest model)
        {
            var exams = await _examAdminService.GetAllPaging(model);
            return Ok(new ResponseBase<PagedResult<Exam>>(data: exams));
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Admin/{id}")]
        public async Task<IActionResult> AdminGetExam(int id)
        {
            var exam = await _examAdminService.GetByID(id);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>(data: exam));
            }
            return Ok(new ResponseBase<Exam>(success: false, msg: "Not found.", data: exam));
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("Admin/{id}")]
        public async Task<IActionResult> AdminDeleteExam(int id)
        {
            var deleted = await _examAdminService.Delete(id);
            if (deleted)
            {
                return Ok(new ResponseBase());
            }
            return Ok(new ResponseBase(success: false, msg: "Failed to delete."));
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Admin/DeleteMany")]
        public async Task<IActionResult> AdminDeleteMany(DeleteManyModel<int> lstExamId)
        {
            var deleted = await _examAdminService.DeleteMany(lstExamId);
            if (deleted)
            {
                return Ok(new ResponseBase());
            }
            return Ok(new ResponseBase(success: false, msg: "Failed to delete."));
        }

        [Authorize(Roles = "admin")]
        [HttpPut("Admin")]
        public async Task<IActionResult> AdminUpdate(ExamModel model)
        {
            var updateResult = await _examAdminService.Update(model);
            if (!updateResult)
            {
                return Ok(new ResponseBase(success: false, msg: "Failed."));
            }
            return Ok(new ResponseBase());
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Admin/Count")]
        public async Task<IActionResult> CountExam()
        {
            return Ok(new ResponseBase<CountExamModel>(data: await _examAdminService.Count()));
        }
        #endregion

        #region User
        [HttpGet]
        public async Task<IActionResult> UserGetAll()
        {
            var exams = await _examUserService.GetAll(GetCurrentUserId());
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }
        [HttpGet("Owned")]
        public async Task<IActionResult> UserGetOwned()
        {
            var exams = await _examUserService.GetOwned(GetCurrentUserId());
            return Ok(new ResponseBase<List<Exam>>(data: exams));
        }
        [HttpPost("Paged")]
        public async Task<IActionResult> UserGetAllPaging(ExamPagingRequest model)
        {
            var exams = await _examUserService.GetAllPaging(model, GetCurrentUserId());
            return Ok(new ResponseBase<PagedResult<Exam>>(data: exams));
        }
        [HttpPost("Paged/Owned")]
        public async Task<IActionResult> UserGetOwnedPaging(ExamPagingRequest model)
        {
            var exams = await _examUserService.GetOwnedPaging(model, GetCurrentUserId());
            return Ok(new ResponseBase<PagedResult<Exam>>(data: exams));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> UserGetOne(int id)
        {
            var exam = await _examUserService.GetByID(id, GetCurrentUserId());
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>(data: exam));
            }
            return Ok(new ResponseBase<Exam>(success: false, msg: "Not found.", data: exam));
        }
        [HttpDelete]
        public async Task<IActionResult> UserDelete([FromQuery] int id)
        {
            var isDeleted = await _examUserService.Delete(id, GetCurrentUserId());
            if (isDeleted)
            {
                return Ok(new ResponseBase());
            }
            return Ok(new ResponseBase(success: false, msg: "Failed."));
        }
        [HttpPost("DeleteMany")]
        public async Task<IActionResult> UserDeleteMany(DeleteManyModel<int> lstExamId)
        {
            var deleted = await _examUserService.DeleteMany(lstExamId);
            if (deleted)
            {
                return Ok(new ResponseBase());
            }
            return Ok(new ResponseBase(success: false, msg: "Failed to delete."));
        }
        [HttpPut]
        public async Task<IActionResult> UserUpdate([FromBody] ExamModel model)
        {
            var updated = await _examUserService.Update(model, GetCurrentUserId());
            if (updated)
            {
                return Ok(new ResponseBase());
            }
            return Ok(new ResponseBase(success: false, msg: "Failed."));
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> Create(ExamModel model)
        {
            var created = await _examUserService.Create(model, GetCurrentUserId());
            if (!created)
            {
                return Ok(new ResponseBase() { success = false, msg = "Failed to create." });
            }
            return Ok(new ResponseBase());
        }

        [HttpPost("IncreaseAttemp/{id}")]
        public async Task<IActionResult> IncreaseAttemp(int id)
        {
            var attemp = await _examUserService.IncreaseAttemps(id);
            return Ok(new ResponseBase<ExamAttemps>(data: new ExamAttemps() { CountAttemps = attemp }));
        }
        //----------------------------------

        private int GetCurrentUserId()
        {
            var check = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (check)
                return userId;
            return -1;
        }
    }
}
