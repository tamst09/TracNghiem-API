using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> AdminGetAll()
        {
            var exams = await _examService.GetAll();
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [Authorize(Roles = "admin")]
        [HttpPost("Paged")]
        public async Task<IActionResult> AdminGetAllPaging(ExamPagingRequest model)
        {
            var exams = await _examService.GetAllPaging(model);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [Authorize(Roles = "admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> AdminGetOne(int id)
        {
            var exam = await _examService.GetByID(id);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> AdminDeleteOne(int id)
        {
            var isDeleted = await _examService.Delete(id);
            if (isDeleted)
            {
                return Ok(new ResponseBase<Exam>() { });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> AdminUpdateOne(int id, Exam model)
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
            return Ok(new ResponseBase<Exam>() { data = model });
        }
        //----------------------------------
        //================================== USER ===================================
        [HttpGet("{userID}")]
        public async Task<IActionResult> UserGetAll(int userID)
        {
            var exams = await _examService.GetAll(userID);
            return Ok(new ResponseBase<List<Exam>>() { data = exams });
        }
        [HttpPost("Paged/{userID}")]
        public async Task<IActionResult> UserGetAllPaging(ExamPagingRequest model, int userID)
        {
            var exams = await _examService.GetAllPaging(model, userID);
            return Ok(new ResponseBase<PagedResult<Exam>>() { data = exams });
        }
        [HttpGet("{id}/{userID}")]
        public async Task<IActionResult> UserGetOne(int id, int userID)
        {
            var exam = await _examService.GetByID(id, userID);
            if (exam != null)
            {
                return Ok(new ResponseBase<Exam>() { data = exam });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không có sẵn" });
        }
        [HttpDelete("{id}/{userID}")]
        public async Task<IActionResult> UserDeleteOne(int id, int userID)
        {
            var isDeleted = await _examService.Delete(id, userID);
            if (isDeleted)
            {
                return Ok(new ResponseBase<Exam>() { });
            }
            return Ok(new ResponseBase<Exam>() { msg = "Đề thi không tồn tại" });
        }
        [HttpPut("{id}/{userID}")]
        public async Task<IActionResult> UserUpdateOne(int id, int userID, Exam model)
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
            return Ok(new ResponseBase<Exam>() { data = model });
        }
        //----------------------------------
        //================================== COMMON =====================================
        [HttpPost("{userID}")]
        public async Task<IActionResult> Create(ExamModel model, int userID)
        {
            var newExam = await _examService.Create(model, userID);
            if(newExam == null)
            {
                return Ok(new ResponseBase<Exam>() { msg = "Người dùng không hợp lệ" });
            }
            return Ok(new ResponseBase<Exam>() { data = newExam });
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
