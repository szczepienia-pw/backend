using System.Diagnostics.CodeAnalysis;
using backend.Dto.Requests.Patient;
using FluentValidation;

namespace backend.Validators.Requests.Patient
{
    [ExcludeFromCodeCoverage]
    public class FilterVaccinationsValidator : AbstractValidator<FilterVaccinationsRequest>
    {
        public FilterVaccinationsValidator()
        {
            RuleFor(r => r.Page).GreaterThanOrEqualTo(1);
        }
    }
}
