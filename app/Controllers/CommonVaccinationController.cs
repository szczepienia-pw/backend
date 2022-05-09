using backend.Helpers;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    public class CommonVaccinationController : ControllerBase
    {
        private readonly CommonVaccinationService vaccinationService;

        public CommonVaccinationController(CommonVaccinationService vaccinationService)
        {
            this.vaccinationService = vaccinationService;
        }

        // GET vaccination-slots
        [HttpGet("vaccination-slots")]
        [Authorize]
        public async Task<IActionResult> GetAvailableVaccinationSlots()
        {
            return Ok(await this.vaccinationService.GetAvailableVaccinationSlots());
        }
    }
}
