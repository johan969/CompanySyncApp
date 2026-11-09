namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency- och användardata för att släppa en RFQ-revision för matching.
/// </summary>
public sealed class ReleaseBuyerRfqRevisionRequest
{
    /// <summary>
    /// Identifierar användaren som släpper revisionen.
    /// </summary>
    public Guid? ReleasedByUserId { get; init; }

    /// <summary>
    /// Concurrency-token för RFQ-roten från senast lästa version.
    /// </summary>
    public uint BuyerRfqRowVersion { get; init; }

    /// <summary>
    /// Concurrency-token för revisionen från senast lästa version.
    /// </summary>
    public uint BuyerRfqRevisionRowVersion { get; init; }
}