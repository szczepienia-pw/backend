using backend.Dto.Requests.Patient;
using FluentValidation;

namespace backend.Validators.Requests.Patient
{
    public class FilterVaccinationsValidator : AbstractValidator<FilterVaccinationsRequest>
    {
        public FilterVaccinationsValidator()
        {
            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        }
    }
}
