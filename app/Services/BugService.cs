using backend.Helpers;
using backend.Database;
using backend.Dto.Requests;
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

        public async Task<int> SendBug(SendBugRequest request, AccountModel account)
        {
            var bugEmail = this.dataContext.Settings.Where(setting => setting.Type == SettingType.BugEmail).First().Value;

            await this.mailer.SendEmailAsync(bugEmail, "Bug report",
                $"Titile: {request.Name}<br>Description: {request.Description}");
            return 1;
        }
    }
}
