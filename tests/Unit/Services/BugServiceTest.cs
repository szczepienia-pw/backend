using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using backend.Dto.Requests;
using backend.Dto.Responses;
using backend.Helpers;
using backend.Models;
using backend.Services;
using backend_tests.Helpers;
using Moq;
using Xunit;

namespace backend_tests.Unit.Services
{
    public class BugServiceTest
    {
        [Fact]
        public async Task TestShouldSendBugReportToSettingEmail()
        {
            var dataContext = DbHelper.GetMockedDataContextWithAccounts().Object;
            var bugMailSetting = dataContext.Settings.Where(setting => setting.Type == SettingType.BugEmail).First();
            var mailerMock = new Mock<Mailer>();
            var senderAccount = dataContext.Doctors.First();

            var service = new BugService(mailerMock.Object, dataContext);
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

            var response = await service.SendBug(request, senderAccount);

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
