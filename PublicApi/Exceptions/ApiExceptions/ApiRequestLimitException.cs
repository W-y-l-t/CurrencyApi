namespace Fuse8.BackendInternship.PublicApi.Exceptions.ApiExceptions;

public class ApiRequestLimitException : Exception
{
    public ApiRequestLimitException(string message = "The API request limit has been exceeded.")
        : base(message)
    { }
}