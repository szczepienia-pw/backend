using backend.Database;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using backend.Dto.Responses;
using backend.Dto.Responses.Doctor.VaccinationSlot;
using backend.Exceptions;
using backend.Helpers;
using backend.Models.Accounts;
using backend.Models.Visits;

namespace backend.Services.Doctor
{
    public class VaccinationSlotService
    {
        private readonly int slotMarginMins = 15;
        private readonly DataContext dataContext;

        public VaccinationSlotService(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<SuccessResponse> AddNewSlot(NewVaccinationSlotRequest request, DoctorModel doctor)
        {
            // Validate date from request
            DateTime date;

            try
            {
                date = DateTime.Parse(request.Date);
            }
            catch (Exception e)
            {
                throw new ValidationException();
            }

            // Check for date in the past
            if(date < DateTime.Now)
                throw new ValidationException("Date is in the past.");

            // Check for overlapping slots
            this.dataContext.VaccinationSlots
                .CheckDuplicate(slot => 
                    slot.Doctor.Id == doctor.Id 
                    && slot.Date > date.AddMinutes(-slotMarginMins) 
                    && slot.Date < date.AddMinutes(slotMarginMins),
                    new ValidationException("Slots overlaps the existing slots.") 
                );

            // Add new slot to database
            VaccinationSlotModel slot = new VaccinationSlotModel { Date = date, Doctor = doctor, Reserved = false };
            this.dataContext.Add(slot);
            this.dataContext.SaveChanges();
            return new SuccessResponse();
        }

        public async Task<PaginatedResponse<VaccinationSlotModel, List<VaccinationSlotResponse>>> GetSlots(FilterVaccinationSlotsRequest request, DoctorModel doctor)
        {
            var slots = this.dataContext.VaccinationSlots.Where(slot => slot.Doctor.Id == doctor.Id);

            if (request.OnlyReserved != null)
                slots = slots.Where(slot => slot.Reserved == Convert.ToBoolean(request.OnlyReserved));
            if (request.StartDate != null)
                slots = slots.Where(slot => slot.Date >= DateTime.Parse(request.StartDate));
            if (request.EndDate != null)
                slots = slots.Where(slot => slot.Date <= DateTime.Parse(request.EndDate));

            var paginatedSlots = PaginatedList<VaccinationSlotModel>.Paginate(slots, request.Page);

            return new PaginatedResponse<VaccinationSlotModel, List<VaccinationSlotResponse>>(
                paginatedSlots, 
                paginatedSlots.Select(slot => new VaccinationSlotResponse(slot)).ToList()
            );
        }

        public async Task<SuccessResponse> DeleteSlot(int vaccinationSlotId, DoctorModel doctor)
        {
            var slot = this.dataContext.VaccinationSlots
                .FirstOrThrow(slot => slot.Id == vaccinationSlotId && slot.Doctor.Id == doctor.Id,
                              new NotFoundException());

            if (slot.Reserved)
                throw new ConflictException("Provided vaccination slot is already reserved and cannot be deleted");

            this.dataContext.VaccinationSlots.Remove(slot);
            this.dataContext.SaveChanges();

            return new SuccessResponse();
        }
    }
}
