using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Controllers.Admin;
using backend.Database;
using backend.Exceptions;
using backend.Services.Admin;
using backend_tests.Helpers;
using Xunit;

namespace backend_tests.Admin
{
    public partial class SettingTest
    {
        private readonly DataContext dataContext;
        private readonly SettingService service;
        private readonly SettingController settingController;

        public SettingTest()
        {
            this.dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            this.service = new SettingService(this.dataContext);
            this.settingController = new SettingController(this.service);
        }

        [Fact]
        public async Task UtTestShouldThrowAnExceptionWhenPassingNotExistingSettingToUpdate()
        {
            var request = new Dictionary<string, string>()
            {
                {"notExistingSetting", "valueForNotExistingSetting"}
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => this.service.Set(request)
            );
        }

        [Fact]
        public async Task UtTestShouldReturnSettingsAsDictionary()
        {
            var response = await this.service.Get();

            Assert.IsType<Dictionary<String, String>>(response);
            foreach (var setting in this.dataContext.Settings)
            {
                var camelCaseType = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(setting.Type.ToString());
                Assert.True(response.ContainsKey(camelCaseType));
                Assert.Equal(response[camelCaseType], setting.Value);
            }
        }
    }
}
