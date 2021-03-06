using Microsoft.AspNetCore.Mvc;
using backend.Helpers;
using backend.Services.Patient;
using backend.Models.Accounts;
using backend.Dto.Requests.Patient;
using backend.Models.Vaccines;
using System.Net.Http.Headers;

namespace backend.Controllers.Patient
{
    [Route("patient")]
    [ApiController]
    public class VaccinationController : ControllerBase
    {
        private readonly PatientVaccinationService vaccinationService;

        public VaccinationController(PatientVaccinationService vaccinationService)
        {
            this.vaccinationService = vaccinationService;
        }

        // GET patient/vaccines
        [HttpGet("vaccines")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> ShowAvailableVaccines([FromQuery] ShowVaccinesRequest request)
        {
            return Ok(await this.vaccinationService.ShowAvailableVaccines(request));
        }

        // PUT patient/vaccination-slots/:{vaccinationSlotId}
        [HttpPut("vaccination-slots/{vaccinationSlotId:int}")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> ReserveVaccinationSlot(int vaccinationSlotId, [FromBody] ReserveSlotRequest request)
        {
            return Ok(await this.vaccinationService.ReserveVaccinationSlot(
                (PatientModel)this.HttpContext.Items["User"], 
                vaccinationSlotId, 
                request.VaccineId
            ));
        }

        // DELETE patient/vaccination-slots/:{vaccinationSlotId}
        [HttpDelete("vaccination-slots/{vaccinationSlotId:int}")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> CancelVaccinationSlot(int vaccinationSlotId)
        {
            return Ok(await this.vaccinationService.CancelVaccinationSlot(
                (PatientModel)this.HttpContext.Items["User"], 
                vaccinationSlotId
            ));
        }
        
        // GET patient/vaccinations
        [HttpGet("vaccinations")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> GetVaccinationsHistory([FromQuery] FilterVaccinationsRequest request)
        {
            return Ok(await this.vaccinationService.GetVaccinationsHistory(
                (PatientModel)this.HttpContext.Items["User"], 
                request
            ));
        }

        // GET patient/vaccinations/:{vaccinationId}/certificate
        [HttpGet("vaccinations/{vaccinationId:int}/certificate")]
        public async Task<IActionResult> DownloadVaccinationCertificate(int vaccinationId)
        {
            // Generate payload
            byte[] payload = this.vaccinationService.DownloadVaccinationCertificate((PatientModel)this.HttpContext.Items["User"], vaccinationId);

            // Return PDF file
            return File(payload, new MediaTypeHeaderValue("application/pdf").ToString());
        }
        
        // DELETE patient/account
        [HttpDelete("account")]
        public async Task<IActionResult> DeletePatient()
        {
            return Ok(await this.vaccinationService.DeletePatient((PatientModel)this.HttpContext.Items["User"]));
        }
    }
}
