using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.Business.Catalog.Interface;
using TN.Data.DataContext;
using TN.Data.Entities;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService=examService;
        }

        // GET: api/Exams
        [HttpGet]
        public async Task<ActionResult<List<Exam>>> GetExams()
        {
            return await _examService.getAll();
        }

        // GET: api/Exams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Exam>> GetExam(int id)
        {
            var exam = await _examService.getByID(id);

            if (exam == null)
            {
                return NotFound();
            }

            return Ok(exam);
        }

        // PUT: api/Exams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExam(int id, Exam exam)
        {

            var result = _examService.update(exam);
            if(result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        // POST api/Exams/userID
        [HttpPost("{userid}")]
        public async Task<ActionResult<Exam>> PostExam(Exam exam, int userid)
        {
            return Ok(await _examService.create(exam, userid));
        }

        // DELETE: api/Exams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteExam(int id)
        {
            var result = await _examService.delete(id);
            if (result == false) return NotFound();
            else return Ok("Delete successfully");
        }
    }
}
