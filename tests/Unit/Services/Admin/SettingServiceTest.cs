using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Database;
using backend.Exceptions;
using backend.Services.Admin;
using backend_tests.Helpers;
using Xunit;

namespace backend_tests.Unit.Services.Admin
{
    public class SettingServiceTest
    {
        private readonly DataContext dataContext;
        private readonly SettingService service;

        public SettingServiceTest()
        {
            this.dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            this.service = new SettingService(this.dataContext);
        }

        [Fact]
        public async Task TestShouldThrowAnExceptionWhenPassingNotExistingSettingToUpdate()
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
        public async Task TestShouldReturnSettingsAsDictionary()
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
