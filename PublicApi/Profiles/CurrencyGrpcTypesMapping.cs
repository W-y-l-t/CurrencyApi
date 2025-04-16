using AutoMapper;
using Fuse8.BackendInternship.PublicApi.Models.Types;

namespace Fuse8.BackendInternship.PublicApi.Profiles;

public class CurrencyGrpcTypesMapping : Profile
{
    public CurrencyGrpcTypesMapping()
    {
        CreateMap<CurrencyCode, InternalApi.Grpc.CurrencyProto.CurrencyCode>().ReverseMap();
    }
}