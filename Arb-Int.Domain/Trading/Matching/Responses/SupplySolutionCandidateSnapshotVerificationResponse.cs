namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller resultatet från verifiering av ett kandidat-snapshot.
/// </summary>
public sealed class SupplySolutionCandidateSnapshotVerificationResponse
{
    /// <summary>
    /// Identifierar kandidaten.
    /// </summary>
    public Guid SupplySolutionCandidateId { get; init; }

    /// <summary>
    /// Anger om det sparade snapshotet matchar den sparade hashen.
    /// </summary>
    public bool IsValid { get; init; }

    /// <summary>
    /// Innehåller den sparade hashen.
    /// </summary>
    public required string StoredHash { get; init; }

    /// <summary>
    /// Innehåller hash beräknad från sparad jsonb.
    /// </summary>
    public required string ComputedHash { get; init; }
}