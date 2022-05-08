using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Admin;
using backend.Helpers;

namespace backend.Controllers.Admin
{
    [Route("admin/")]
    [ApiController]
    public class AdminAuthController : ControllerBase
    {
        private readonly AdminAuthService authService;

        public AdminAuthController(AdminAuthService authService)
        {
            this.authService = authService;
        }

        // POST admin/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest request)
        {
            return Ok(await this.authService.Authenticate(request));
        }
    }
}
