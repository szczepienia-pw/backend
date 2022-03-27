using backend.Dto.Requests.Doctor.VaccinationSlot;
using FluentValidation;

namespace backend.Validators.Requests.Doctor.VaccinationSlot
{
    public class FilterVaccinationSlotValidator : AbstractValidator<FilterVaccinationSlotsRequest>
    {
        public FilterVaccinationSlotValidator()
        {
            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        }
    }
}
