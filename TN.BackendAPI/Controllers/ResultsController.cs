using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TN.BackendAPI.Services.IServices;
using TN.ViewModels.Catalog.Result;

namespace TN.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService _resultService;

        public ResultsController(IResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(AddResultRequest addResultRequest)
        {
            var res = await _resultService.AddResult(addResultRequest, GetCurrentUserId());
            return Ok(res);
        }

        [HttpPost("AddList")]
        public async Task<IActionResult> AddList(AddListResultRequest addListResultRequest)
        {
            var res = await _resultService.AddListResult(addListResultRequest, GetCurrentUserId());
            return Ok(res);
        }

        private int GetCurrentUserId()
        {
            var check = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);
            if (check)
                return userId;
            return -1;
        }
    }
}
