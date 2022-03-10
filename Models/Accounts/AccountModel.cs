using System.Text.Json.Serialization;

namespace backend.Models.Accounts
{
    public abstract class AccountModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
