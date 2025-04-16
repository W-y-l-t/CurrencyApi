using Fuse8.BackendInternship.InternalApi.Contracts.Messages;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Fuse8.BackendInternship.InternalApi.Interceptors;

public class ValidateInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, 
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        if (request is not IValidatable validatableRequest)
            return await continuation(request, context);

        try
        {
            validatableRequest.Validate();
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        
        return await continuation(request, context);
    }
}