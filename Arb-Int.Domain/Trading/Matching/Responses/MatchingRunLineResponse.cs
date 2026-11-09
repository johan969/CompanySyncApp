namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller det frysta radunderlaget för en matchningskörning.
/// </summary>
public sealed class MatchingRunLineResponse
{
    /// <summary>
    /// Identifierar run-raden.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar matchningskörningen.
    /// </summary>
    public Guid MatchingRunId { get; init; }

    /// <summary>
    /// Identifierar RFQ-revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Identifierar den ursprungliga RFQ-raden.
    /// </summary>
    public Guid BuyerRfqLineId { get; init; }

    /// <summary>
    /// RFQ-radens nummer.
    /// </summary>
    public int BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// RFQ-radens namn.
    /// </summary>
    public required string BuyerRfqLineName { get; init; }

    /// <summary>
    /// Fryst efterfrågad kvantitet.
    /// </summary>
    public decimal RequiredQuantity { get; init; }

    /// <summary>
    /// Identifierar fryst kvantitetsenhet.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Canonical JSON för radens input vid körningens start.
    /// </summary>
    public string? InputSnapshotJson { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}