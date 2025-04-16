namespace Fuse8.BackendInternship.InternalApi.Middlewares;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogInformation(
            "Incoming request: {Method} {Url} Query: {Query}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString);
        
        await next(context);
        
        _logger.LogInformation(
            "Response headers: {Headers} Response body: {Body}", 
            context.Response.Headers,
            context.Response.Body);
    }
}