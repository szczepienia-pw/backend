namespace backend.Models.Vaccines
{
    public class Vaccine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Disease Disease { get; set; }
        public int RequiredDoses { get; set; }
        public string SerialNumber { get; set; }
    }
}
