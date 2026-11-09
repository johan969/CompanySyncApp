namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Skapar och verifierar canonical snapshots för släppta RFQ-revisioner.
/// </summary>
public interface IBuyerRfqRevisionSnapshotService
{
    /// <summary>
    /// Bygger canonical JSON och SHA-256-hash för en RFQ-revision.
    /// </summary>
    Task<BuyerRfqRevisionSnapshotPayload> CreateAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Beräknar canonical SHA-256-hash för ett redan sparat JSON-snapshot.
    /// </summary>
    string ComputeHash(string snapshotJson);
}

/// <summary>
/// Innehåller canonical JSON och tillhörande hash.
/// </summary>
public sealed class BuyerRfqRevisionSnapshotPayload
{
    /// <summary>
    /// Canonical JSON som ska sparas på revisionen.
    /// </summary>
    public required string SnapshotJson { get; init; }

    /// <summary>
    /// SHA-256-hash beräknad från canonical JSON.
    /// </summary>
    public required string SnapshotHash { get; init; }
}