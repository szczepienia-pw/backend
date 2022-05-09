using System.Collections.Generic;
using backend.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Admin;

public partial class SettingTest
{
    [Fact]
    public void ItSuccessfullyGetSettings()
    {
        var rsp = this.settingController.GetSettings().Result;

        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    }

    [Theory]
    [InlineData("test", "test")]
    public void ItFailureToSetSettings(string setting, string value)
    {
        Assert.ThrowsAsync<NotFoundException>(() =>
            this.settingController.SetSettings(new Dictionary<string, string>() { { setting, value } }));
    } 
}