using backend.Models.Visits;

namespace backend.Dto.Responses.Patient.Vaccination
{
    public class AvailableSlotResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public AvailableSlotResponse(VaccinationSlotModel vaccinationSlotModel)
        {
            this.Id = vaccinationSlotModel.Id;
            this.Date = vaccinationSlotModel.Date.ToUniversalTime();
        }
    }
}

