using AutoMapper;
using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Profiles;

public class CurrencyGrpcTypesMapping : Profile
{
    public CurrencyGrpcTypesMapping()
    {
        CreateMap<CurrencyCode, Grpc.CurrencyProto.CurrencyCode>().ReverseMap();
    }
}