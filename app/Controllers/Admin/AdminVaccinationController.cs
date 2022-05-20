using backend.Dto.Requests;
using backend.Dto.Requests.Admin;
using backend.Helpers;
using backend.Services.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

        // GET admin/vaccinations
        [HttpGet]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> GetVaccinations([FromQuery] FilterVaccinationsRequest request)
        {
            return Ok(await this.adminVaccinationService.GetVaccinations(request));
        }
        
        // GET admin/vaccinations/report
        [HttpGet("report")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> GetVaccinationsReport([FromQuery] VaccinationsReportRequest request)
        {
            return Ok(await this.adminVaccinationService.GetVaccinationsReport(request));
        }
        
        // GET admin/vaccinations/report/download
        [HttpGet("report/download")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> DownloadVaccinationsReport([FromQuery] VaccinationsReportRequest request)
        {
            // Generate payload
            byte[] payload = this.adminVaccinationService.DownloadVaccinationsReport(request);

            // Return PDF file
            return File(payload, new MediaTypeHeaderValue("application/pdf").ToString());
        }
    }
}