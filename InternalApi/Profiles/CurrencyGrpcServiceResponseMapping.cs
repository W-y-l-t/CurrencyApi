using AutoMapper;
using Fuse8.BackendInternship.InternalApi.Grpc.CurrencyProto;
using Fuse8.BackendInternship.InternalApi.Models.DataTransferObjects;

namespace Fuse8.BackendInternship.InternalApi.Profiles;

public class CurrencyGrpcServiceResponseMapping : Profile
{
    public CurrencyGrpcServiceResponseMapping()
    {
        CreateMap<CurrencyRateDto, CurrencyResponse>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate));

        CreateMap<MinimalSettingsDto, SettingsResponse>()
            .ForMember(dest => dest.NewRequestAvailable, opt => opt.MapFrom(src => src.NewRequestsAvailable));
    }
}