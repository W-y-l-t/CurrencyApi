namespace Fuse8.BackendInternship.PublicApi.Exceptions.DataBaseExceptions;

public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message = "Data have not found") 
        : base(message)
    { }
}