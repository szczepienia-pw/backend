using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Exceptions;
using backend.Services.Admin;
using backend_tests.Helpers;
using Xunit;

namespace backend_tests.Unit.Services.Admin
{
    public class SettingServiceTest
    {
        [Fact]
        public async Task TestShouldThrowAnExceptionWhenPassingNotExistingSettingToUpdate()
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var service = new SettingService(dataContext);
            var request = new Dictionary<string, string>()
            {
                {"notExistingSetting", "valueForNotExistingSetting"}
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => service.Set(request)
            );
        }

        [Fact]
        public async Task TestShouldReturnSettingsAsDictionary()
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var service = new SettingService(dataContext);

            var response = await service.Get();

            Assert.IsType<Dictionary<String, String>>(response);
            foreach (var setting in dataContext.Settings)
            {
                var camelCaseType = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(setting.Type.ToString());
                Assert.True(response.ContainsKey(camelCaseType));
                Assert.Equal(response[camelCaseType], setting.Value);
            }
        }
    }
}
