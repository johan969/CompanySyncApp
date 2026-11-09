namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller resultatet av integritetskontrollen för ett sparat revisionssnapshot.
/// </summary>
public sealed class BuyerRfqSnapshotVerificationResponse
{
    /// <summary>
    /// Identifierar revisionen.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Anger om det sparade snapshotet fortfarande motsvarar den sparade hashen.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Hash som finns sparad på revisionen.
    /// </summary>
    public required string StoredHash { get; init; }

    /// <summary>
    /// Hash som beräknades från det sparade snapshotet.
    /// </summary>
    public required string ComputedHash { get; init; }
}