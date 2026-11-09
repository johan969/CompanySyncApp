namespace Arb_Int.Domain.Location;

/// <summary>
/// Representerar full referensdata för ett land.
/// </summary>
public sealed class CountryResponse
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
    /// Anger standardtidzon när sådan finns.
    /// </summary>
    public string? DefaultTimeZone { get; init; }

    /// <summary>
    /// Anger ISO 3166 alpha-2-koden.
    /// </summary>
    public string? Alpha2Code { get; init; }

    /// <summary>
    /// Anger ISO 3166 alpha-3-koden.
    /// </summary>
    public string? Alpha3Code { get; init; }

    /// <summary>
    /// Anger ISO 3166 numerisk kod.
    /// </summary>
    public string? ISO3166Numeric { get; init; }

    /// <summary>
    /// Anger standardmoms när sådan finns.
    /// </summary>
    public double? DefaultVAT { get; init; }

    /// <summary>
    /// Anger standardvalutans id när sådan finns.
    /// </summary>
    public int? CurrencyId { get; init; }

    /// <summary>
    /// Anger standardvalutans ISO-kod när sådan finns.
    /// </summary>
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Anger standardvalutans namn när sådan finns.
    /// </summary>
    public string? CurrencyName { get; init; }

    /// <summary>
    /// Anger kontinentens id.
    /// </summary>
    public int ContinentId { get; init; }

    /// <summary>
    /// Anger kontinentens namn.
    /// </summary>
    public required string ContinentName { get; init; }

    /// <summary>
    /// Anger kontinentens kod.
    /// </summary>
    public required string ContinentCode { get; init; }
}
