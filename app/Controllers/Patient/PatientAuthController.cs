using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Patient;
using backend.Helpers;

namespace backend.Controllers.Patient
{
    [Route("patient")]
    [ApiController]
    public class PatientAuthController : ControllerBase
    {
        private readonly PatientAuthService authService;

        public PatientAuthController(PatientAuthService authService)
        {
            this.authService = authService;
        }

        // POST patient/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest request)
        {
            return Ok(await this.authService.Authenticate(request));
        }
    }
}
