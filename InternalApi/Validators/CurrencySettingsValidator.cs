using FluentValidation;
using Fuse8.BackendInternship.InternalApi.Settings;

namespace Fuse8.BackendInternship.InternalApi.Validators;

public class CurrencySettingsValidator : AbstractValidator<CurrencySettings>
{
    public CurrencySettingsValidator()
    {
        RuleFor(x => x.CurrencyRoundCount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("CurrencyRoundCount must be >= 0");   
    }
}