using backend.Models.Visits;

namespace backend.Dto.Responses.Doctor.VaccinationSlot
{
    public class VaccinationSlotResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public object Vaccination { get; set; }

        public VaccinationSlotResponse(VaccinationSlotModel vaccinationSlotModel)
        {
            this.Id = vaccinationSlotModel.Id;
            this.Date = vaccinationSlotModel.Date;
            this.Vaccination = new { };
        }
    }
}
