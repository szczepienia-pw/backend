using backend.Helpers;
using backend.Database;
using backend.Dto.Requests;
using backend.Dto.Responses;
using backend.Models;
using backend.Models.Accounts;

namespace backend.Services
{
    public class BugService
    {
        private readonly Mailer mailer;
        private readonly DataContext dataContext;

        public BugService(Mailer mailer, DataContext dataContext)
        {
            this.mailer = mailer;
            this.dataContext = dataContext;
        }

        public async Task<SuccessResponse> SendBug(SendBugRequest request, AccountModel account)
        {
            var bugEmail = this.dataContext.Settings.Where(setting => setting.Type == SettingType.BugEmail).First().Value;

            _ = Task.Factory.StartNew(async () =>
            {
                await this.mailer.SendEmailAsync(
                    bugEmail,
                    "Bug report",
                    $"Titile: {request.Name}<br>Description: {request.Description}<br>Sent by: {account.FirstName} {account.LastName} - {account.Email} ({account.GetEnum().ToString()})"
                );
            });

            return new SuccessResponse();
        }
    }
}
