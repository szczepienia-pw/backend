using backend.Dto.Requests.Doctor.VaccinationSlot;
using FluentValidation;

namespace backend.Validators.Requests.Doctor.VaccinationSlot
{
    public class FilterVaccinationValidator : AbstractValidator<FilterVaccinationSlotsRequest>
    {
        public FilterVaccinationValidator()
        {
            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        }
    }
}
