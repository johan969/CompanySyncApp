using System.Linq.Expressions;

namespace Arb_Int.Domain.Economy.Projections;

/// <summary>
/// Innehåller återanvändbara projektioner för valutor.
/// </summary>
public static class CurrencyProjections
{
    /// <summary>
    /// Projekterar en valuta till referensrespons.
    /// </summary>
    public static Expression<Func<Currency, CurrencyResponse>> Response => x => new CurrencyResponse
    {
        Id = x.Id,
        Name = x.Name,
        Code = x.Code,
        Symbol = x.Symbol,
        Number = x.Number,
        DecimalDigits = x.DecimalDigits,
        ExchangeRate = x.ExchangeRate
    };
}
