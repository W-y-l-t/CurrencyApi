using AutoMapper;
using Fuse8.BackendInternship.InternalApi.Contracts.Services;
using Fuse8.BackendInternship.InternalApi.Grpc.CurrencyProto;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using CurrencyCode = Fuse8.BackendInternship.InternalApi.Models.Types.CurrencyCode;
using DateOnly = System.DateOnly;

namespace Fuse8.BackendInternship.InternalApi.Services.GrpcServices;

public class CurrencyGrpcService : CurrencyService.CurrencyServiceBase
{
    private readonly ICachedCurrencyApiService _cachedCurrencyApiService;
    private readonly IMapper _mapper;

    public CurrencyGrpcService(ICachedCurrencyApiService cachedCurrencyApiService, IMapper mapper)
    {
        _cachedCurrencyApiService = cachedCurrencyApiService;
        _mapper = mapper;
    }
    
    public override async Task<CurrencyResponse> GetCurrency(CurrencyRequest request, ServerCallContext context)
    {
        var baseCurrencyCode = _mapper.Map<CurrencyCode>(request.BaseCode);
        var currencyCode = _mapper.Map<CurrencyCode>(request.Code);
        
        var result = await _cachedCurrencyApiService.GetCurrentCurrencyAsync(
            baseCurrencyCode, 
            currencyCode, 
            context.CancellationToken);
        
        var response = _mapper.Map<CurrencyResponse>(result);
        
        return response;
    }

    public override async Task<CurrencyResponse> GetCurrencyOnDate(
        CurrencyOnDateRequest request,
        ServerCallContext context)
    {
        var baseCurrencyCode = _mapper.Map<CurrencyCode>(request.BaseCode);
        var currencyCode = _mapper.Map<CurrencyCode>(request.Code);
        var date = new DateOnly(request.Date.Year, request.Date.Month, request.Date.Day);
        
        var result = await _cachedCurrencyApiService.GetCurrencyOnDateAsync(
            baseCurrencyCode, 
            currencyCode, 
            date, 
            context.CancellationToken);
        
        var response = _mapper.Map<CurrencyResponse>(result);
        
        return response;
    }

    public override async Task<SettingsResponse> GetSettings(Empty request, ServerCallContext context)
    {
        var settings = await _cachedCurrencyApiService.GetSettingsAsync(context.CancellationToken);
        
        var response = _mapper.Map<SettingsResponse>(settings);
        
        return response;   
    }
}