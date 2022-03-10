namespace backend.Models.Visits
{
    public class VaccinationSlot
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Reserved { get; set; }
    }
}
