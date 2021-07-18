using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.Data.Entities;
using TN.ViewModels.Catalog.Question;
using TN.ViewModels.Common;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // GET: api/Questions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allQuestions = await _questionService.GetAll();
            if(allQuestions != null)
                return Ok(new ResponseBase<List<Question>>() { data = allQuestions });
            return Ok(new ResponseBase<List<Question>>() { msg = "Lỗi hệ thống" });
        }
        // GET: api/Questions/GetNumber
        [HttpGet("GetNumber")]
        public async Task<IActionResult> GetNumberQuestion()
        {
            var result = await _questionService.CountQuestions();
            return Ok(new ResponseBase<string>() { data = result.ToString() });
        }
        // POST: api/Questions
        [HttpPost]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            var createOK = await _questionService.Create(model);
            if (createOK != null)
                return Ok(new ResponseBase<Question>() { data = createOK });
            return Ok(new ResponseBase<Question>() { msg = "Lỗi hệ thống" });
        }
        // POST: api/Questions/Paged
        [HttpPost("Paged")]
        public async Task<IActionResult> GetAllPaging(QuestionPagingRequest model)
        {
            var allQuestions = await _questionService.GetAllPaging(model);
            if (allQuestions != null)
                return Ok(new ResponseBase<PagedResult<Question>>() { data = allQuestions });
            return Ok(new ResponseBase<PagedResult<Question>>() { msg = "Lỗi hệ thống" });
        }
        // GET: api/Questions/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var question = await _questionService.GetByID(id);
            if (question != null)
                return Ok(new ResponseBase<Question>() { data = question });
            return Ok(new ResponseBase<Question>() { msg = "Không tìm thấy" });
        }
        // PUT: api/Questions/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuestionModel model)
        {
            if (id != model.ID)
            {
                return Ok(new ResponseBase<Question>() { msg = "Không hợp lệ" });
            }
            var ok = await _questionService.Update(model);
            if (ok)
                return Ok(new ResponseBase<string>() { data = "Cập nhật thành công" });
            return Ok(new ResponseBase<string>() { msg = "Không tìm thấy" });
        }
        // DELETE: api/Questions/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            var ok = await _questionService.Delete(id);
            if (ok)
            {
                return Ok(new ResponseBase<string>() { data = "Xoá thành công" });
            }
            return Ok(new ResponseBase<string>() { msg = "Không tìm thấy" });
        }
        // POST: api/Questions/DeleteMany
        [HttpPost("DeleteMany")]
        public async Task<IActionResult> DeleteMany(DeleteManyModel<int> lstId)
        {
            var ok = await _questionService.DeleteMany(lstId);
            if (ok)
            {
                return Ok(new ResponseBase<string>() { data = "Xoá thành công "});
            }
            return Ok(new ResponseBase<string>() { msg = "Xoá thất bại" });
        }

        [HttpGet("Count")]
        [Authorize("admin")]
        public async Task<IActionResult> Count()
        {
            return Ok(new ResponseBase<int>(data: await _questionService.CountQuestions()));
        }
    }
}
