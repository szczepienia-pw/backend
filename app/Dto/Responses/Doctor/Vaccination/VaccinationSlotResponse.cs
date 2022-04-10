using backend.Models.Visits;

namespace backend.Dto.Responses.Doctor.Vaccination
{
    public class VaccinationSlotResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public object Vaccination { get; set; }

        public VaccinationSlotResponse(VaccinationSlotModel vaccinationSlotModel, VaccinationModel? vaccination)
        {
            this.Id = vaccinationSlotModel.Id;
            this.Date = vaccinationSlotModel.Date;
            if (vaccination == null) this.Vaccination = new { };
            else this.Vaccination = new VaccinationResponse(vaccination);
        }
    }
}
