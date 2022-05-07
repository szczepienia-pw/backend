using System.Diagnostics.CodeAnalysis;
using backend.Dto.Requests.Doctor.VaccinationSlot;
using FluentValidation;

namespace backend.Validators.Requests.Doctor.VaccinationSlot
{
    [ExcludeFromCodeCoverage]
    public class FilterVaccinationValidator : AbstractValidator<FilterVaccinationSlotsRequest>
    {
        public FilterVaccinationValidator()
        {
            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        }
    }
}
