using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Controllers;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses;
using backend.Helpers;
using backend.Models;
using backend.Services;
using backend_tests.Helpers;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace backend_tests.Bug
{
    public partial class BugTest
    {
        private readonly Mock<DataContext> dbContext;
        private readonly Mock<Mailer> mailerMock;
        private readonly BugService bugService;
        private readonly BugController bugController;
        
        public BugTest()
        {
            this.dbContext = DbHelper.GetMockedDataContextWithAccounts();
            this.mailerMock = new Mock<Mailer>();
            this.bugService = new BugService(this.mailerMock.Object, this.dbContext.Object);
            this.bugController = new BugController(this.bugService);

            this.bugController.ControllerContext.HttpContext = new DefaultHttpContext();

            var doctor = this.dbContext.Object.Doctors.First(d => d.Id == 1);
            this.bugController.ControllerContext.HttpContext.Items.Add("User", doctor);
        }
        
        [Fact]
        public async Task UtTestShouldSendBugReportToSettingEmail()
        {
            var bugMailSetting = this.dbContext.Object.Settings.Where(setting => setting.Type == SettingType.BugEmail).First();
            var senderAccount = this.dbContext.Object.Doctors.First();

            var request = new SendBugRequest()
            {
                Name = Faker.Name.FullName(),
                Description = Faker.Company.Name()
            };

            mailerMock.Setup(mailer => mailer.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(), 
                It.IsAny<string>(),
                null
            ));
            mailerMock.Setup(mailer => mailer.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null)).Returns(Task.FromResult(Task.CompletedTask));

            var response = await this.bugService.SendBug(request, senderAccount);

            Assert.IsType<SuccessResponse>(response);

            // Verify if mailer has been called with correct params
            mailerMock.Verify(mailer => mailer.SendEmailAsync(
                bugMailSetting.Value,
                "Bug report",
                It.Is<string>(
                    body => body.Contains(request.Name) 
                            && body.Contains(request.Description)
                            && body.Contains(senderAccount.FirstName)
                            && body.Contains(senderAccount.LastName)
                            && body.Contains(senderAccount.Email)
                            && body.Contains(senderAccount.GetEnum().ToString())
                    ),
                null
            ), Times.Once);
        }
    }
}
