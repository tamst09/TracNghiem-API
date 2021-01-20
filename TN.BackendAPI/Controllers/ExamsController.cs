using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.DataContext;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Category;
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
        // GET: api/Exams/Admin/
        [Authorize(Roles = "admin")]
        [HttpGet("Admin")]
        public async Task<IActionResult> AdminGetAll()
        {
            var exams = await _examService.GetAll();
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }

        // POST: api/Exams/Admin/Paged
        [Authorize(Roles = "admin")]
        [HttpPost("Admin/Paged")]
        public async Task<IActionResult> AdminGetAllPaging(ExamPagingRequest model)
        {
            var exams = await _examService.GetAllPaging(model);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }

        // GET: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpGet("Admin/{id}")]
        public async Task<IActionResult> AdminGetOne(int id)
        {
            var exam = await _examService.GetByID(id);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }

        // DELETE: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpDelete("Admin/{id}")]
        public async Task<IActionResult> AdminDeleteOne(int id)
        {
            var isDeleted = await _examService.Delete(id);
            if (isDeleted)
            {
                return Ok(new ResponseBase<string>() { });
            }
            return Ok(new ResponseBase<string>() { msg = "Đề thi không có sẵn" });
        }

        // POST: api/Exams/Admin/DeleteMany
        [Authorize(Roles = "admin")]
        [HttpPost("Admin/DeleteMany")]
        public async Task<IActionResult> AdminDeleteMany(DeleteRangeModel<int> lstExamId)
        {
            var isDeleted = await _examService.DeleteMany(lstExamId);
            if (isDeleted)
            {
                return Ok(new ResponseBase<string>() { });
            }
            return Ok(new ResponseBase<string>() { msg = "Xoá thất bại" });
        }

        // PUT: api/Exams/Admin/1
        [Authorize(Roles = "admin")]
        [HttpPut("Admin/{id}")]
        public async Task<IActionResult> AdminUpdateOne(int id, ExamModel model)
        {
            if(id != model.ID)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Đề thi không tồn tại" });
            }
            var updateResult = await _examService.Update(model);
            if (!updateResult)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Lỗi cập nhật. Thử lại sau." });
            }
            return Ok(new ResponseBase<Exam>() { });
        }
        //----------------------------------
        //================================== USER ===================================
        [HttpGet("{userID}")]
        public async Task<IActionResult> UserGetAll(int userID)
        {
            var exams = await _examService.GetAll(userID);
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> UserGetOwned([FromQuery]int userID)
        {
            var exams = await _examService.GetOwned(userID);
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [HttpPost("Paged")]
        public async Task<IActionResult> UserGetAllPaging(ExamPagingRequest model,[FromQuery] int userID)
        {
            var exams = await _examService.GetAllPaging(model, userID);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [HttpPost("Paged/Owned")]
        public async Task<IActionResult> UserGetOwnedPaging(ExamPagingRequest model, [FromQuery] int userID)
        {
            var exams = await _examService.GetOwnedPaging(model, userID);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [HttpGet("User")]
        public async Task<IActionResult> UserGetOne([FromQuery]int id, [FromQuery]int userID)
        {
            var exam = await _examService.GetByID(id, userID);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }
        [HttpDelete("User")]
        public async Task<IActionResult> UserDeleteOne([FromQuery]int id, [FromQuery]int userID)
        {
            var isDeleted = await _examService.Delete(id, userID);
            if (isDeleted)
            {
                return Ok(new ResponseBase<Exam>() { });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không tồn tại" });
        }
        [HttpPut("User")]
        public async Task<IActionResult> UserUpdateOne([FromQuery]int id, [FromQuery]int userID, [FromBody]ExamModel model)
        {
            if (id != model.ID)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Đề thi không hợp lệ" });
            }
            var updateResult = await _examService.Update(model, userID);
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
            var newExam = await _examService.Create(model, userID);
            if(newExam == null)
            {
                return Ok(new ResponseBase<Exam>() { success = false, msg = "Người dùng không hợp lệ" });
            }
            return Ok(new ResponseBase<Exam>() { success = true, data = newExam });
        }
        [HttpPost("IncreaseAttemp/{id}")]
        public async Task<IActionResult> IncreaseAttemp(int id)
        {
            var attemp = await _examService.IncreaseAttemps(id);
            return Ok(new ResponseBase<int>() { data = attemp });
        }
        //----------------------------------
    }
}
