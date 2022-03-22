using backend.Dto.Requests;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    public class BugController : ControllerBase
    {
        private readonly BugService bugService;

        public BugController(BugService bugService)
        {
            this.bugService = bugService;
        }

        // POST bugs
        [HttpPost("bugs")]
        [Authorize]
        public async Task<IActionResult> Login([FromBody] SendBugRequest request)
        {
            return Ok(await this.bugService.SendBug(request, (AccountModel)this.HttpContext.Items["User"]));
        }
    }
}
