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
            return Ok(await _questionService.GetAll());
        }
        // GET: api/Questions/GetNumber
        [HttpGet("GetNumber")]
        public async Task<IActionResult> GetNumberQuestion()
        {
            return Ok(await _questionService.GetAll());
        }
        // POST: api/Questions
        [HttpPost]
        public async Task<IActionResult> Create(QuestionModel model)
        {
            return Ok(await _questionService.Create(model));
        }
        // POST: api/Questions/Paged
        [HttpPost("Paged")]
        public async Task<IActionResult> GetAllPaging(QuestionPagingRequest model)
        {
            return Ok(await _questionService.GetAllPaging(model));
        }
        // GET: api/Questions/1
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            return Ok(await _questionService.GetOne(id));
        }
        // PUT: api/Questions/1
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, QuestionModel model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }
            return Ok(await _questionService.Update(model));
        }
        // DELETE: api/Questions/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOne(int id)
        {
            return Ok(await _questionService.Delete(id));
        }
        // POST: api/Questions/DeleteMany
        [HttpPost("DeleteMany")]
        public async Task<IActionResult> AdminDeleteMany(DeleteRangeModel<int> lstId)
        {
            return Ok(await _questionService.DeleteMany(lstId));
        }
    }
}
