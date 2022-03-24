using System.Diagnostics;
using backend.Database;
using backend.Dto.Responses;
using backend.Exceptions;

namespace backend.Services.Admin
{
    public class SettingService
    {
        private readonly DataContext dataContext;

        public SettingService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        private string ToCamelCase(string text)
        {
            return System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(text);
        }

        public async Task<Dictionary<String, String>> Get()
        {
            var settings = this.dataContext.Settings;
            var responseDictionary = new Dictionary<String, String>();
            
            foreach (var setting in settings)
                responseDictionary.Add(
                    this.ToCamelCase(setting.Type.ToString()), 
                    setting.Value
                );

            return responseDictionary;
        }

        public async Task<SuccessResponse> Set(Dictionary<String, String> request)
        {
            var settings = this.dataContext.Settings;

            foreach (var requestKey in request.Keys)
            {
                bool foundSetting = false;

                foreach (var setting in settings)
                {
                    if (this.ToCamelCase(setting.Type.ToString()) == requestKey)
                    {
                        foundSetting = true;
                        setting.Value = request[requestKey];
                    }
                }

                if (!foundSetting)
                    throw new ValidationException();
            }

            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }
    }
}
