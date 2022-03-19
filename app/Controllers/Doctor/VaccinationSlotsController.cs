using Microsoft.AspNetCore.Mvc;
using backend.Services.Doctor;
using backend.Helpers;
using backend.Dto.Requests.Doctor;
using backend.Models.Accounts;

namespace backend.Controllers.Doctor
{
    [Route("doctor/")]
    [ApiController]
    public class VaccinationSlotsController : ControllerBase
    {
        private readonly VaccinationSlotService vaccinationSlotService;

        public VaccinationSlotsController(VaccinationSlotService vaccinationSlotService)
        {
            this.vaccinationSlotService = vaccinationSlotService;
        }

        // POST doctor/vaccination-slots
        [HttpPost("vaccination-slots")]
        [Authorize(AccountTypeEnum.Doctor)]
        public async Task<IActionResult> AddVaccinationSlots([FromBody] NewVaccinationSlotRequest request)
        {
            return Ok(await this.vaccinationSlotService.AddNewSlot(request, (DoctorModel)this.HttpContext.Items["User"]));
        }
    }
}
