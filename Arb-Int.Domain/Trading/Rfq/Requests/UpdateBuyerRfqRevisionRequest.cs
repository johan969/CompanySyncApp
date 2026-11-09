namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbar metadata för en draft-revision.
/// </summary>
public sealed class UpdateBuyerRfqRevisionRequest
{
    /// <summary>
    /// Sammanfattning av revisionen.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Köparens ursprungliga underlag.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Intern tolkning av underlaget.
    /// </summary>
    public string? InternalInterpretation { get; init; }

    /// <summary>
    /// Tidpunkt då svar önskas.
    /// </summary>
    public DateTimeOffset? ResponseRequestedByUtc { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa revision.
    /// </summary>
    public uint RowVersion { get; init; }
}