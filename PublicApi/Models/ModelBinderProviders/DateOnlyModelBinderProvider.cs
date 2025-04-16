using Fuse8.BackendInternship.PublicApi.Models.ModelBinders;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fuse8.BackendInternship.PublicApi.Models.ModelBinderProviders;

public class DateOnlyModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        return context.Metadata.ModelType == typeof(DateOnly) ? new DateOnlyModelBinder() : null;
    }
}