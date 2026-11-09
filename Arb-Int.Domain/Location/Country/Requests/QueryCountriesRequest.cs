namespace Arb_Int.Domain.Location;

/// <summary>
/// Anger filter för uppslag av länder.
/// </summary>
public sealed class QueryCountriesRequest
{
    /// <summary>
    /// Anger fritextsökning mot namn och ISO-koder.
    /// </summary>
    public string? Search { get; init; }

    /// <summary>
    /// Begränsar resultatet till en kontinent.
    /// </summary>
    public int? ContinentId { get; init; }

    /// <summary>
    /// Begränsar resultatet till EU- eller icke-EU-länder.
    /// </summary>
    public bool? IsEUCountry { get; init; }

    /// <summary>
    /// Begränsar resultatet till en valuta.
    /// </summary>
    public int? CurrencyId { get; init; }
}
