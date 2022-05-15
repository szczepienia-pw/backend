using backend.Models.Visits;

namespace backend.Dto.Responses.Admin.Vaccination
{
    public class VaccinationSlotResponse
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }

        public VaccinationSlotResponse(VaccinationSlotModel vaccinationSlotModel)
        {
            this.Id = vaccinationSlotModel.Id;
            this.Date = vaccinationSlotModel.Date.ToUniversalTime();
        }
    }
}
