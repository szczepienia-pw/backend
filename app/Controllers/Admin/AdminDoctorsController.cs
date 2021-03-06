using Microsoft.AspNetCore.Mvc;
using backend.Helpers;
using backend.Services.Admin;
using backend.Dto.Requests.Admin;
using backend.Dto.Requests.Admin.Doctor;
using backend.Dto.Requests;

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

        //PUT admin/doctors/:doctor-id
        [HttpPut("{doctorId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> UpdateDoctor(int doctorId, [FromBody] EditDoctorRequest request)
        {
            return Ok(await this.doctorsService.EditDoctor(doctorId, request));
        }

        // GET admin/doctors/:doctor-id
        [HttpGet("{doctorId:int}")]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> ShowDoctor(int doctorId)
        {
            return Ok(await this.doctorsService.ShowDoctor(doctorId));
        }

        // GET admin/doctors
        [HttpGet()]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> ShowDoctors([FromQuery] PaginationRequest request)
        {
            return Ok(await this.doctorsService.ShowDoctors(request.Page));
        }
    }
}