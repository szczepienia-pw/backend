using backend.Dto.Requests;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace backend_tests.Bug;

public partial class BugTest
{
    [Fact]
    public void SuccessfullySendBug()
    {
        var rsp = this.bugController.SendBug(new SendBugRequest() {Name = "test", Description = "test"}).Result;
        
        Assert.IsType<OkObjectResult>(rsp);
        Assert.Equal(200, ((OkObjectResult) rsp).StatusCode);
    } 
}