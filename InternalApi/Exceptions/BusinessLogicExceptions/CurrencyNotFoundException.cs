namespace Fuse8.BackendInternship.InternalApi.Exceptions.BusinessLogicExceptions;

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string message = "Currency not found.")
        : base(message)
    { }
}