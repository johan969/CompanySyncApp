namespace Arb_Int.Domain.Location;

/// <summary>
/// Representerar ett land i listor och uppslagsfält.
/// </summary>
public sealed class CountryListResponse
{
    /// <summary>
    /// Anger landets id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Anger landets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Anger om landet är medlem i EU.
    /// </summary>
    public bool IsEUCountry { get; init; }

    /// <summary>
    /// Anger ISO 3166 alpha-2-koden.
    /// </summary>
    public string? Alpha2Code { get; init; }

    /// <summary>
    /// Anger ISO 3166 alpha-3-koden.
    /// </summary>
    public string? Alpha3Code { get; init; }

    /// <summary>
    /// Anger kontinentens id.
    /// </summary>
    public int ContinentId { get; init; }

    /// <summary>
    /// Anger kontinentens namn.
    /// </summary>
    public required string ContinentName { get; init; }

    /// <summary>
    /// Anger landets standardvaluta när sådan finns.
    /// </summary>
    public int? CurrencyId { get; init; }

    /// <summary>
    /// Anger valutans ISO-kod när valuta finns.
    /// </summary>
    public string? CurrencyCode { get; init; }
}
