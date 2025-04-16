using FluentValidation;
using Microsoft.Extensions.Options;

namespace Fuse8.BackendInternship.PublicApi.Extensions.ValidationExtensions;

public static class OptionsBuilderExtension
{
    public static OptionsBuilder<TOptions> ValidateFluent<TOptions, TValidator>(
        this OptionsBuilder<TOptions> builder)
        where TOptions : class
        where TValidator : class, IValidator<TOptions>
    {
        builder.Services.AddSingleton<IValidateOptions<TOptions>>(sp =>
        {
            var validator = sp.GetRequiredService<TValidator>();
            
            return new FluentValidationOptions<TOptions>(validator);
        });

        return builder;
    }

    private class FluentValidationOptions<TOptions> : IValidateOptions<TOptions>
        where TOptions : class
    {
        private readonly IValidator<TOptions> _validator;

        public FluentValidationOptions(IValidator<TOptions> validator)
        {
            _validator = validator;
        }

        public ValidateOptionsResult Validate(string? name, TOptions options)
        {
            var result = _validator.Validate(options);
            if (result.IsValid)
            {
                return ValidateOptionsResult.Success;
            }

            var errors = result.Errors.Select(f => $"[{f.PropertyName}] {f.ErrorMessage}");
            
            return ValidateOptionsResult.Fail(errors);
        }
    }
}