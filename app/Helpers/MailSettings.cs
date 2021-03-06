using System.Diagnostics.CodeAnalysis;

namespace backend.Helpers
{
    [ExcludeFromCodeCoverage]
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string DisplayEmail { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
