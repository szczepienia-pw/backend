using Microsoft.AspNetCore.Mvc;
using backend.Helpers;
using backend.Services.Patient;
using backend.Models.Accounts;
using backend.Dto.Requests.Patient;
using backend.Models.Vaccines;

namespace backend.Controllers.Patient
{
    [Route("patient")]
    [ApiController]
    public class VaccinationController : ControllerBase
    {
        private readonly VaccinationService vaccinationService;

        public VaccinationController(VaccinationService vaccinationService)
        {
            this.vaccinationService = vaccinationService;
        }

        // GET patient/vaccines
        [HttpGet("vaccines")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> ShowAvailableVaccines([FromQuery] ShowVaccinesRequest request)
        {
            return Ok(await this.vaccinationService.ShowAvailableVaccines(DiseaseEnumAdapter.ToEnum(request.Disease)));
        }

        // GET patient/vaccination-slots
        [HttpGet("vaccination-slots")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> GetAvailableVaccinationSlots()
        {
            return Ok(await this.vaccinationService.GetAvailableVaccinationSlots());
        }

        //PUT patient/vaccination-slots/:{vaccinationSlotId}
        [HttpPut("vaccination-slots/{vaccinationSlotId:int}")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> ReserveVaccinationSlot(int vaccinationSlotId, [FromBody] ReserveSlotRequest request)
        {
            return Ok(await this.vaccinationService.ReserveVaccinationSlot((PatientModel)this.HttpContext.Items["User"], vaccinationSlotId, request.VaccineId));
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
    }
}
