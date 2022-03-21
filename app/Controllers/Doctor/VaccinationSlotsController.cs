using Microsoft.AspNetCore.Mvc;
using backend.Services.Doctor;
using backend.Helpers;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Models.Accounts;

namespace backend.Controllers.Doctor
{
    [Route("doctor/vaccination-slots")]
    [ApiController]
    public class VaccinationSlotsController : ControllerBase
    {
        private readonly VaccinationSlotService vaccinationSlotService;

        public VaccinationSlotsController(VaccinationSlotService vaccinationSlotService)
        {
            this.vaccinationSlotService = vaccinationSlotService;
        }

        // POST doctor/vaccination-slots
        [HttpPost]
        [Authorize(AccountTypeEnum.Doctor)]
        public async Task<IActionResult> AddVaccinationSlot([FromBody] NewVaccinationSlotRequest request)
        {
            return Ok(await this.vaccinationSlotService.AddNewSlot(request, (DoctorModel)this.HttpContext.Items["User"]));
        }

        // GET doctor/vaccination-slots
        [HttpGet]
        [Authorize(AccountTypeEnum.Doctor)]
        public async Task<IActionResult> GetVaccinationSlots([FromQuery] FilterVaccinationSlotsRequest request)
        {
            return Ok(await this.vaccinationSlotService.GetSlots(request, (DoctorModel)this.HttpContext.Items["User"]));
        }
    }
}
