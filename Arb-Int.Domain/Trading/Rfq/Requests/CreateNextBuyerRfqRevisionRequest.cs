namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller concurrency-data för att skapa nästa draft-revision från aktuell released revision.
/// </summary>
public sealed class CreateNextBuyerRfqRevisionRequest
{
    /// <summary>
    /// Concurrency-token för RFQ-roten från senast lästa version.
    /// </summary>
    public uint BuyerRfqRowVersion { get; init; }
}