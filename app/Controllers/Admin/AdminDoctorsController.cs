using Microsoft.AspNetCore.Mvc;
using backend.Services.Admin;
using backend.Helpers;

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

        // DELETE admin/doctors/:doctor-id
        [HttpDelete("{doctorId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            return Ok(await this.doctorsService.DeleteDoctor(doctorId));
        }
    }
}