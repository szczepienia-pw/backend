using backend.Dto.Requests.Patient;
using backend.Helpers;
using backend.Services.Admin;
using backend.Services.Patient;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Admin
{
    [Route("admin/patients")]
    [ApiController]
    public class AdminPatientController : ControllerBase
    {
        private readonly PatientService patientService;

        public AdminPatientController(PatientService patientService)
        {
            this.patientService = patientService;
        }

        // PUT admin/patients/:patient-id
        [HttpPut("{patientId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> EditPatient(int patientId, [FromBody] PatientRequest request)
        {
            return Ok(await this.patientService.EditPatient(patientId, request));
        }
    }
}
