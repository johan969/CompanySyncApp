using System.Linq.Expressions;

namespace Arb_Int.Domain.Location.Projections;

/// <summary>
/// Innehåller återanvändbara projektioner för länder.
/// </summary>
public static class CountryProjections
{
    /// <summary>
    /// Projekterar ett land till en kompakt listrespons.
    /// </summary>
    public static Expression<Func<Country, CountryListResponse>> List => x => new CountryListResponse
    {
        Id = x.Id,
        Name = x.Name,
        IsEUCountry = x.IsEUCountry,
        Alpha2Code = x.Alpha2Code,
        Alpha3Code = x.Alpha3Code,
        ContinentId = x.ContinentId,
        ContinentName = x.Continent.Name,
        CurrencyId = x.CurrencyId,
        CurrencyCode = x.Currency != null ? x.Currency.Code : null
    };

    /// <summary>
    /// Projekterar ett land till full referensrespons.
    /// </summary>
    public static Expression<Func<Country, CountryResponse>> Response => x => new CountryResponse
    {
        Id = x.Id,
        Name = x.Name,
        IsEUCountry = x.IsEUCountry,
        DefaultTimeZone = x.DefaultTimeZone,
        Alpha2Code = x.Alpha2Code,
        Alpha3Code = x.Alpha3Code,
        ISO3166Numeric = x.ISO3166Numeric,
        DefaultVAT = x.DefaultVAT,
        CurrencyId = x.CurrencyId,
        CurrencyCode = x.Currency != null ? x.Currency.Code : null,
        CurrencyName = x.Currency != null ? x.Currency.Name : null,
        ContinentId = x.ContinentId,
        ContinentName = x.Continent.Name,
        ContinentCode = x.Continent.ContinentCode
    };
}
