using System.ComponentModel.DataAnnotations;
using Fuse8.BackendInternship.InternalApi.Models.Types;

namespace Fuse8.BackendInternship.InternalApi.Models.Entities;

/// <summary>
/// Сущность для хранения кэшированного курса валюты.
/// Для текущих курсов HistoricalDate == null, 
/// для исторических – HistoricalDate содержит дату курса.
/// </summary>
public class CurrencyCacheEntry
{
    [Key]
    public int Id { get; set; }
    
    public CurrencyCode BaseCurrency { get; set; }
        
    public CurrencyCode Currency { get; set; }
        
    public decimal Rate { get; set; }
        
    public DateTime CachedAt { get; set; }
    
    public DateOnly TargetDate { get; set; }
}