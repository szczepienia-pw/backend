using backend.Dto.Requests;
using backend.Helpers;
using backend.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Admin
{
    [Route("admin/vaccinations")]
    [ApiController]
    public class AdminVaccinationController : ControllerBase
    {
        private readonly AdminVaccinationService adminVaccinationService;

        public AdminVaccinationController(AdminVaccinationService adminVaccinationService)
        {
            this.adminVaccinationService = adminVaccinationService;
        }

        // POST admin/vaccinations/:vaccination-id/change-slot
        [HttpPost("{vaccinationId:int}/change-slot")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> ChangeSlot(int vaccinationId, [FromBody] ChangeSlotRequest request)
        {
            return Ok(await this.adminVaccinationService.ChangeVaccinationSlot(vaccinationId, request.VaccinationSlotId));
        }
    }
}