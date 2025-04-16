using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Exceptions.DataBaseExceptions;

public class DataNotFoundException : Exception
{
    public DataNotFoundException(params CurrencyCode[] args)
        : base(BuildMessage(args)) 
    { }

    private static string BuildMessage(CurrencyCode[] args)
    {
        if (args.Length == 0)
        {
            return "Couldn't get the cache for the requested currencies.";
        }

        var message = "Couldn't get the cache for the requested currencies:";
        for (var i = 0; i < args.Length; i++)
        {
            message += $" {args[i]}";
            message += i == args.Length - 1 ? "." : ",";
        }

        return message;
    }
}