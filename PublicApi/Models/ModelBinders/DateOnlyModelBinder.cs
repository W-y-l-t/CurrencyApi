using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fuse8.BackendInternship.PublicApi.Models.ModelBinders;

public class DateOnlyModelBinder : IModelBinder
{
    private const string DateFormat = "yyyy-MM-dd";
    
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        
        var date = valueProviderResult.FirstValue;
        
        if (DateOnly.TryParseExact(
                date, 
                DateFormat, 
                CultureInfo.InvariantCulture, 
                DateTimeStyles.None, 
                out var dateOnly))
        {
            bindingContext.Result = ModelBindingResult.Success(dateOnly);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(
                bindingContext.ModelName, $"Date format must be {DateFormat}.");
        }
        
        return Task.CompletedTask;
    }
}