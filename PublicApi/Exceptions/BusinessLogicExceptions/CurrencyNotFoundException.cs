namespace Fuse8.BackendInternship.PublicApi.Exceptions.BusinessLogicExceptions;

public class CurrencyNotFoundException : Exception
{
    public CurrencyNotFoundException(string message = "Currency not found.")
        : base(message)
    { }
}