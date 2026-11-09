using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Bygger immutable snapshots av färdigberäknade supply-solution-candidates.
/// </summary>
public interface ISupplySolutionCandidateSnapshotService
{
    /// <summary>
    /// Bygger canonical JSON och SHA-256 för kandidatens maskinella resultat.
    /// </summary>
    Task<CanonicalJsonPayload> CreateAsync(Guid supplySolutionCandidateId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Beräknar hash från ett redan lagrat kandidat-snapshot.
    /// </summary>
    string ComputeHash(string snapshotJson);
}