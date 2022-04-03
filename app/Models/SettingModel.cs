namespace backend.Models
{
    public class SettingModel : BaseModel
    {
        public int Id { get; set; }
        public SettingType Type { get; set; }
        public string Value { get; set; } = "";
    }

    public enum SettingType
    {
        BugEmail
    }
}
