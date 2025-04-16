using Fuse8.BackendInternship.InternalApi.Contracts.Messages;
using Grpc.Core;

// ReSharper disable once CheckNamespace
namespace Fuse8.BackendInternship.InternalApi.Grpc.CurrencyProto;

public partial class CurrencyRequest : IValidatable
{
    public void Validate()
    {
        if (Code == CurrencyCode.Unknown)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Currency"));
        }
    }
}

public partial class CurrencyOnDateRequest : IValidatable
{
    public void Validate()
    {
        if (Code == CurrencyCode.Unknown)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Currency"));
        
        if (Date is null)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Date is required"));
        
        if (Date.Year < 1 || Date.Month < 1 || Date.Month > 12 || Date.Day < 1 || Date.Day > 31)
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Date"));
    }
}