using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Patient;
using backend.Helpers;
using backend.Dto.Requests.Patient;

namespace backend.Controllers.Patient
{
    [Route("patient/registration")]
    [ApiController]
    public class PatientRegistrationController : ControllerBase
    {
        private readonly PatientRegistrationService registrationService;

        public PatientRegistrationController(PatientRegistrationService registrationService)
        {
            this.registrationService = registrationService;
        }

        // POST patient/registration
        [HttpPost]
        public async Task<IActionResult> Registration([FromBody] PatientRegistrationRequest request)
        {
            return Ok(await this.registrationService.Register(request));
        }
    }
}
