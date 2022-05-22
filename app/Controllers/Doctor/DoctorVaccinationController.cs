using backend.Dto.Requests;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Services.Doctor;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Doctor
{
    [Route("doctor/vaccinations")]
    [ApiController]
    public class DoctorVaccinationController : ControllerBase
    {
        private readonly DoctorVaccinationService doctorVaccinationService;

        public DoctorVaccinationController(DoctorVaccinationService doctorVaccinationService)
        {
            this.doctorVaccinationService = doctorVaccinationService;
        }

        // POST doctor/vaccinations/:vaccination-id/change-slot
        [HttpPost("{vaccinationId:int}/change-slot")]
        [Authorize(AccountTypeEnum.Doctor)]
        public async Task<IActionResult> ChangeSlot(int vaccinationId, [FromBody] ChangeSlotRequest request)
        {
            return Ok(await this.doctorVaccinationService.ChangeVaccinationSlot(vaccinationId, request.VaccinationSlotId, (DoctorModel)this.HttpContext.Items["User"]));
        }
    }
}