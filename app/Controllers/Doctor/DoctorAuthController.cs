using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Doctor;
using backend.Helpers;

namespace backend.Controllers.Doctor
{
    [Route("doctor")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly DoctorAuthService authService;

        public DoctorAuthController(DoctorAuthService authService)
        {
            this.authService = authService;
        }

        // POST doctor/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest request)
        {
            return Ok(await this.authService.Authenticate(request));
        }
    }
}
