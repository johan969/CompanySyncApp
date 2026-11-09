namespace Arb_Int.Domain.Economy;

/// <summary>
/// Representerar en valuta som referensdata.
/// </summary>
public sealed class CurrencyResponse
{
    /// <summary>
    /// Anger valutans id.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Anger valutans namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger ISO 4217-koden.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Anger valutasymbolen när sådan finns.
    /// </summary>
    public string? Symbol { get; init; }

    /// <summary>
    /// Anger ISO 4217 numerisk kod när sådan finns.
    /// </summary>
    public string? Number { get; init; }

    /// <summary>
    /// Anger normalt antal decimaler när värdet finns.
    /// </summary>
    public int? DecimalDigits { get; init; }

    /// <summary>
    /// Anger aktuell växelkurs mot EUR när sådan finns.
    /// </summary>
    public decimal? ExchangeRate { get; init; }
}
