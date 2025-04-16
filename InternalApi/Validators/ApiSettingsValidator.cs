using FluentValidation;
using Fuse8.BackendInternship.InternalApi.Settings;

namespace Fuse8.BackendInternship.InternalApi.Validators;

public class ApiSettingsValidator : AbstractValidator<ApiSettings>
{
    public ApiSettingsValidator()
    {
        RuleFor(x => x.ApiKey)
            .NotEmpty()
            .WithMessage("ApiKey is required");

        RuleFor(x => x.BaseUrl)
            .NotEmpty()
            .WithMessage("BaseUrl is required");
    }
}