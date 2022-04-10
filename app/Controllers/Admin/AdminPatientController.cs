using backend.Dto.Requests.Admin.Patient;
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
        private readonly AdminPatientsService adminPatientsService;

        public AdminPatientController(PatientService patientService, AdminPatientsService adminPatientsService)
        {
            this.patientService = patientService;
            this.adminPatientsService = adminPatientsService;
        }

        // GET admin/patients
        [HttpGet]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> ShowPatients([FromBody] ShowPatientsRequest request)
        {
            return Ok(await this.adminPatientsService.ShowPatients(request.Page));
        }

        // GET admin/patients/:patient-id
        [HttpGet("{patientId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> ShowPatient(int patientId)
        {
            return Ok(await this.adminPatientsService.ShowPatient(patientId));
        }

        // PUT admin/patients/:patient-id
        [HttpPut("{patientId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> EditPatient(int patientId, [FromBody] PatientRequest request)
        {
            return Ok(await this.patientService.EditPatient(patientId, request));
        }

        // DELETE admin/patients/:patient-id
        [HttpDelete("{patientId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> DeletePatient(int patientId)
        {
            return Ok(await this.adminPatientsService.DeletePatient(patientId));
        }
    }
}
