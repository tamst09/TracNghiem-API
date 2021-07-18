using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        //=============================== ADMIN ===================================
        [Authorize(Roles = "admin")]
        [HttpGet("Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> AdminGetExams()
        {
            var exams = await _examAdminService.GetAll();
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Admin/Paged")]
        public async Task<IActionResult> AdminGetExamsPaged(ExamPagingRequest model)
        {
            var exams = await _examAdminService.GetAllPaging(model);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Admin/{id}")]
        public async Task<IActionResult> AdminGetExam(int id)
        {
            var exam = await _examAdminService.GetByID(id);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("Admin/{id}")]
        public async Task<IActionResult> AdminDeleteExam(int id)
        {
            var isDeleted = await _examAdminService.Delete(id);
            if (isDeleted)
            {
                return Ok(new ResponseBase<string>() { });
            }
            return Ok(new ResponseBase<string>() { msg = "Đề thi không có sẵn" });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Admin/DeleteMany")]
        public async Task<IActionResult> AdminDeleteMany(DeleteManyModel<int> lstExamId)
        {
            var isDeleted = await _examAdminService.DeleteMany(lstExamId);
            if (isDeleted)
            {
                return Ok(new ResponseBase<string>() { });
            }
            return Ok(new ResponseBase<string>() { msg = "Xoá thất bại" });
        }

        [Authorize(Roles = "admin")]
        [HttpPut("Admin/{id}")]
        public async Task<IActionResult> AdminUpdate(int id, ExamModel model)
        {
            if(id != model.ID)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Đề thi không tồn tại" });
            }
            var updateResult = await _examAdminService.Update(model);
            if (!updateResult)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Lỗi cập nhật. Thử lại sau." });
            }
            return Ok(new ResponseBase<Exam>() { });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Admin/Count")]
        public async Task<IActionResult> CountExam()
        {
            return Ok(new ResponseBase<int>(data: await _examAdminService.Count()));
        }
        //----------------------------------
        //================================== USER ===================================
        [HttpGet("{userID}")]
        public async Task<IActionResult> UserGetAll(int userID)
        {
            var exams = await _examUserService.GetAll(userID);
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [HttpGet("Owned/{userID}")]
        public async Task<IActionResult> UserGetOwned(int userID)
        {
            var exams = await _examUserService.GetOwned(userID);
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [HttpPost("Paged")]
        public async Task<IActionResult> UserGetAllPaging(ExamPagingRequest model,[FromQuery] int userID)
        {
            var exams = await _examUserService.GetAllPaging(model, userID);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [HttpPost("Paged/Owned")]
        public async Task<IActionResult> UserGetOwnedPaging(ExamPagingRequest model, [FromQuery] int userID)
        {
            var exams = await _examUserService.GetOwnedPaging(model, userID);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [HttpGet]
        public async Task<IActionResult> UserGetOne([FromQuery]int id, [FromQuery]int userID)
        {
            var exam = await _examUserService.GetByID(id, userID);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }
        [HttpDelete]
        public async Task<IActionResult> UserDelete([FromQuery]int id, [FromQuery]int userID)
        {
            var isDeleted = await _examUserService.Delete(id, userID);
            if (isDeleted)
            {
                return Ok(new ResponseBase<Exam>() { });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không tồn tại" });
        }
        [HttpPut]
        public async Task<IActionResult> UserUpdateOne([FromQuery]int id, [FromQuery]int userID, [FromBody]ExamModel model)
        {
            if (id != model.ID)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Đề thi không hợp lệ" });
            }
            var updateResult = await _examUserService.Update(model, userID);
            if (!updateResult)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Lỗi cập nhật" });
            }
            return Ok(new ResponseBase<Exam>() { });
        }

        //----------------------------------
        //================================== COMMON =====================================
        [HttpPost]
        public async Task<IActionResult> Create(ExamModel model, [FromQuery]int userID)
        {
            var newExam = await _examUserService.Create(model, userID);
            if(newExam == null)
            {
                return Ok(new ResponseBase<Exam>() { success = false, msg = "Người dùng không hợp lệ" });
            }
            return Ok(new ResponseBase<Exam>() { success = true, data = newExam });
        }
        [HttpPost("IncreaseAttemp/{id}")]
        public async Task<IActionResult> IncreaseAttemp(int id)
        {
            var attemp = await _examUserService.IncreaseAttemps(id);
            return Ok(new ResponseBase<int>() { data = attemp });
        }
        //----------------------------------
    }
}
