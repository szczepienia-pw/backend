using Microsoft.AspNetCore.Mvc;
using backend.Dto.Requests;
using backend.Services.Admin;
using backend.Helpers;

namespace backend.Controllers.Admin
{
    [Route("admin/settings")]
    [ApiController]
    public class SettingController : ControllerBase
    {
        private readonly SettingService settingService;

        public SettingController(SettingService settingService)
        {
            this.settingService = settingService;
        }


        // GET admin/settings
        [HttpGet()]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> GetSettings()
        {
            return Ok(await this.settingService.Get());
        }

        // PUT admin/settings
        [HttpPut()]
        [Authorize(AccountTypeEnum.Admin)]
        public async Task<IActionResult> SetSettings([FromBody] Dictionary<String, String> request)
        {
            return Ok(await this.settingService.Set(request));
        }
    }
}
