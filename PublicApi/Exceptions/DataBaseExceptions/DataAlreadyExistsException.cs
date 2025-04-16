namespace Fuse8.BackendInternship.PublicApi.Exceptions.DataBaseExceptions;

public class DataAlreadyExistsException : Exception
{
    public DataAlreadyExistsException(string message = "Data already exists.") 
        : base(message)
    { }
}