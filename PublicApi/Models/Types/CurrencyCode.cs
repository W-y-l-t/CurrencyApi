namespace Fuse8.BackendInternship.PublicApi.Models.Types;

/// <summary>
/// Перечисление кодов валют
/// </summary>
public enum CurrencyCode
{
    /// <summary>Неизвестный код</summary>
    Unknown = 0,
    
    /// <summary>Доллар США</summary>
    Usd = 1,
    
    /// <summary>Российский рубль</summary>
    Rub = 2,
    
    /// <summary>Казахстанский тенге</summary>
    Kzt = 3,
    
    /// <summary>Евро</summary>
    Eur = 4,
    
    /// <summary>Британский фунт стерлингов</summary>
    Gbp = 5,
    
    /// <summary>Японская иена</summary>
    Jpy = 6,
    
    /// <summary>Китайский юань</summary>
    Cny = 7,
    
    /// <summary>Швейцарский франк</summary>
    Chf = 8,
    
    /// <summary>Индийская рупия</summary>
    Inr = 9,
    
    /// <summary>Австралийский доллар</summary>
    Aud = 10
}