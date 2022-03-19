﻿using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Doctor;
using backend.Helpers;
using backend.Dto.Requests.Doctor;
using backend.Models.Visits;
using backend.Models.Accounts;

namespace backend.Controllers.Doctor
{
    [Route("doctor/")]
    [ApiController]
    public class DoctorAuthController : ControllerBase
    {
        private readonly DoctorAuthService authService;

        public DoctorAuthController(DoctorAuthService authService)
        {
            this.authService = authService;
        }

        // POST doctor/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateRequest request)
        {
            return Ok(await this.authService.Authenticate(request));
        }

        // GET doctor/auth-test
        [HttpGet("auth-test")]
        [Authorize(AccountTypeEnum.Doctor)]
        public async Task<IActionResult> AuthTest()
        {
            return Ok();
        }
    }
}
