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
            return Ok(new ResponseBase<List<Question>>(data: allQuestions));
        }

        // GET: api/Questions/GetByExam
        [HttpPost("GetByExam")]
        public async Task<IActionResult> GetByExam([FromBody] GetQuestionsByExamRequest request)
        {
            var questionsResponse = await _questionService.GetByExam(request);
            return Ok(questionsResponse);
        }

        // GET: api/Questions/Count
        [HttpGet("Count")]
        public async Task<IActionResult> CountQuestion()
        {
            var result = await _questionService.CountQuestions();
            return Ok(new ResponseBase<CountQuestionModel>(data: new CountQuestionModel() { NumberQuestions = result }));
        }
        // POST: api/Questions
        [HttpPost]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            var created = await _questionService.Create(model);
            if (created)
                return Ok(new ResponseBase());
            return Ok(new ResponseBase(success: false, msg: "Failed to create."));
        }
        // POST: api/Questions/Paged
        [HttpPost("Paged")]
        public async Task<IActionResult> GetAllPaging(QuestionPagingRequest model)
        {
            var allQuestions = await _questionService.GetAllPaging(model);
            return Ok(new ResponseBase<PagedResult<Question>>(data: allQuestions));
        }
        // GET: api/Questions/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var question = await _questionService.GetByID(id);
            if (question != null)
                return Ok(new ResponseBase<Question>(data: question));
            return Ok(new ResponseBase<Question>(success: false, msg: "Not found.", data: question));
        }
        // PUT: api/Questions
        [HttpPut]
        public async Task<IActionResult> Update(QuestionModel model)
        {
            var updated = await _questionService.Update(model);
            if (updated)
                return Ok(new ResponseBase());
            return Ok(new ResponseBase(success: false, msg: "Not found."));
        }
        // DELETE: api/Questions/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            var deleted = await _questionService.Delete(id);
            if (deleted)
                return Ok(new ResponseBase());
            return Ok(new ResponseBase(success: false, msg: "Not found."));
        }
        // POST: api/Questions/DeleteMany
        [HttpPost("DeleteMany")]
        public async Task<IActionResult> DeleteMany(DeleteManyModel<int> lstId)
        {

            var deleted = await _questionService.DeleteMany(lstId);
            if (deleted)
                return Ok(new ResponseBase());
            return Ok(new ResponseBase(success: false, msg: "Delete failed."));
        }

        [HttpGet("Count")]
        [Authorize("admin")]
        public async Task<IActionResult> Count()
        {
            return Ok(new ResponseBase<int>(data: await _questionService.CountQuestions()));
        }
    }
}
