using FluentValidation;
using Fuse8.BackendInternship.PublicApi.Settings;

namespace Fuse8.BackendInternship.PublicApi.Validators;

public class GrpcSettingsValidator : AbstractValidator<GrpcSettings>
{
    public GrpcSettingsValidator()
    {
        RuleFor(x => x.InternalApiUrl)
            .NotEmpty();
    }
}