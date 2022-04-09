using Microsoft.AspNetCore.Mvc;
using backend.Helpers;
using backend.Services.Patient;
using backend.Models.Accounts;
using backend.Dto.Requests.Patient;

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

        //PUT patient/vaccination-slots/:{vaccinationSlotId}
        [HttpPut("vaccination-slots/{vaccinationSlotId:int}")]
        [Authorize(AccountTypeEnum.Patient)]
        public async Task<IActionResult> ReserveVaccinationSlot(int vaccinationSlotId, [FromBody] ReserveSlotRequest request)
        {
            return Ok(await this.vaccinationService.ReserveVaccinationSlot((PatientModel)this.HttpContext.Items["User"], vaccinationSlotId, request.VaccineId));
        }
    }
}
