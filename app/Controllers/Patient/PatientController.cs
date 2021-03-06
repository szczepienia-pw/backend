using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Patient;
using backend.Helpers;
using backend.Dto.Requests.Patient;
using backend.Dto.Responses.Patient;
using backend.Models.Accounts;

namespace backend.Controllers.Patient
{
    [Route("patient")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService patientService;

        public PatientController(PatientService patientService)
        {
            this.patientService = patientService;
        }

        // POST patient/registration
        [HttpPost("registration")]
        public async Task<IActionResult> Registration([FromBody] PatientRegistrationRequest request)
        {
            return Ok(await this.patientService.Register(request));
        }
        
        // POST patient/registration/confirm
        [HttpPost("registration/confirm")]
        public async Task<IActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationRequest request)
        {
            return Ok(await this.patientService.ConfirmRegistration(request));
        }

        // PUT patient/account
        [HttpPut("account")]
        public async Task<IActionResult> EditPatient([FromBody] PatientRequest request)
        {
            return Ok(await this.patientService.EditPatient((PatientModel)this.HttpContext.Items["User"], request));
        }
    }
}
