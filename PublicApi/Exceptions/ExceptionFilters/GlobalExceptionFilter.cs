using Fuse8.BackendInternship.PublicApi.Exceptions.ApiExceptions;
using Fuse8.BackendInternship.PublicApi.Exceptions.BusinessLogicExceptions;
using Fuse8.BackendInternship.PublicApi.Exceptions.DataBaseExceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fuse8.BackendInternship.PublicApi.Exceptions.ExceptionFilters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }
    
    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled)
            return;
        
        ProblemDetails problemDetails;

        switch (context.Exception)
        {
            case ApiRequestLimitException:
                _logger.LogInformation(context.Exception.Message);

                problemDetails =
                    ConfigureProblemDetails("Request limit reached.", StatusCodes.Status429TooManyRequests);
            
                break;
            
            case CurrencyNotFoundException:
                problemDetails = 
                    ConfigureProblemDetails("Currency not found.", StatusCodes.Status422UnprocessableEntity);
                
                break;
            
            case DataAlreadyExistsException:
                _logger.LogInformation(context.Exception.Message);
    
                problemDetails =
                    ConfigureProblemDetails("Data already exists.", StatusCodes.Status417ExpectationFailed);
                
                break;
            
            case DataNotFoundException:
                _logger.LogInformation(context.Exception.Message);
                
                problemDetails = 
                    ConfigureProblemDetails("Data does not exist.", StatusCodes.Status424FailedDependency);
                
                break;
            
            case RpcException exception:
                problemDetails = new ProblemDetails
                {
                    Title = "RPC exception.",
                    Detail = exception.Message,
                    Status = StatusCodes.Status503ServiceUnavailable
                };
                
                break;
            
            default:
                _logger.LogError(context.Exception, "An error occurred during the execution of the request");
            
                problemDetails = 
                    ConfigureProblemDetails("Internal server error.", StatusCodes.Status500InternalServerError);
                
                break;
        }
        
        context.Result = new JsonResult(problemDetails);
        context.HttpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status501NotImplemented;
        context.ExceptionHandled = true;
    }

    private static ProblemDetails ConfigureProblemDetails(string title, int statusCode)
    {
        return new ProblemDetails
        {
            Title = title,
            Status = statusCode
        };
    }
}