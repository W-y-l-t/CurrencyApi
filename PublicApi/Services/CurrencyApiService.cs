using AutoMapper;
using Fuse8.BackendInternship.InternalApi.Grpc.CurrencyProto;
using Fuse8.BackendInternship.PublicApi.Contracts.Services;
using Fuse8.BackendInternship.PublicApi.Models.DataTransferObjects;
using Fuse8.BackendInternship.PublicApi.Settings;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using CurrencyCode = Fuse8.BackendInternship.PublicApi.Models.Types.CurrencyCode;
using DateOnly = System.DateOnly;

namespace Fuse8.BackendInternship.PublicApi.Services;

public class CurrencyApiService : ICurrencyApiService
{
    private readonly CurrencyService.CurrencyServiceClient _grpcClient;
    private readonly IOptionsSnapshot<CurrencySettings> _currencySettingsSnapshot;
    private readonly IMapper _mapper;

    public CurrencyApiService(
        CurrencyService.CurrencyServiceClient grpcClient,
        IOptionsSnapshot<CurrencySettings> currencySettingsSnapshot,
        IMapper mapper)
    {
        _grpcClient = grpcClient;
        _currencySettingsSnapshot = currencySettingsSnapshot;
        _mapper = mapper;
    }
    
    public async Task<SettingsDto> GetSettingsAsync(CancellationToken cancellationToken)
    {
        var grpcResponse = await _grpcClient.GetSettingsAsync(new Empty(), cancellationToken: cancellationToken);
        
        var currencySettings = _currencySettingsSnapshot.Value;
        
        return new SettingsDto(grpcResponse.NewRequestAvailable, currencySettings.CurrencyRoundCount);
    }

    public async Task<CurrencyRateDto> GetCurrencyRateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        CancellationToken cancellationToken)
    {
        var grpcResponse = await _grpcClient.GetCurrencyAsync(
            new CurrencyRequest
            {
                BaseCode = _mapper.Map<InternalApi.Grpc.CurrencyProto.CurrencyCode>(baseCurrencyCode),
                Code = _mapper.Map<InternalApi.Grpc.CurrencyProto.CurrencyCode>(currencyCode)
            },
            cancellationToken: cancellationToken
        );

        var currencySettings = _currencySettingsSnapshot.Value;
        var decimalRate = (decimal)grpcResponse.Rate;
        
        var roundedValue = RoundValue(decimalRate, currencySettings.CurrencyRoundCount);

        return new CurrencyRateDto(currencyCode, roundedValue);
    }

    public async Task<CurrencyRateOnDateDto> GetCurrencyRateOnDateAsync(
        CurrencyCode baseCurrencyCode,
        CurrencyCode currencyCode, 
        DateOnly date,
        CancellationToken cancellationToken)
    {
        var grpcDate = new InternalApi.Grpc.CurrencyProto.DateOnly
        {
            Year = date.Year,
            Month = date.Month,
            Day = date.Day
        };

        var grpcResponse = await _grpcClient.GetCurrencyOnDateAsync(
            new CurrencyOnDateRequest 
            { 
                BaseCode = _mapper.Map<InternalApi.Grpc.CurrencyProto.CurrencyCode>(baseCurrencyCode),
                Code = _mapper.Map<InternalApi.Grpc.CurrencyProto.CurrencyCode>(currencyCode), 
                Date = grpcDate 
            },
            cancellationToken: cancellationToken
        );

        var currencySettings = _currencySettingsSnapshot.Value;
        var decimalRate = (decimal)grpcResponse.Rate;
        
        var roundedValue = RoundValue(decimalRate, currencySettings.CurrencyRoundCount);

        return new CurrencyRateOnDateDto(
            currencyCode,
            roundedValue,
            date
        );
    }

    private static decimal RoundValue(decimal value, int precision)
    {
        return Math.Round(value, precision);
    }
}