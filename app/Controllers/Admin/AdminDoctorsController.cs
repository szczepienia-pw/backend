using Microsoft.AspNetCore.Mvc;
using backend.Helpers;
using backend.Services.Admin;
using backend.Dto.Requests.Admin;

namespace backend.Controllers.Admin
{
    [Route("admin/doctors")]
    [ApiController]
    public class AdminDoctorsController : ControllerBase
    {
        private readonly AdminDoctorsService doctorsService;

        public AdminDoctorsController(AdminDoctorsService doctorsService)
        {
            this.doctorsService = doctorsService;
        }

        // POST admin/doctors
        [HttpPost]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorRequest request)
        {
            return Ok(await this.doctorsService.CreateDoctor(request));
        }

        // DELETE admin/doctors/:doctor-id
        [HttpDelete("{doctorId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            return Ok(await this.doctorsService.DeleteDoctor(doctorId));
        }
    }
}