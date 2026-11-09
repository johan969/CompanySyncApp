using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Bygger och verifierar buyer-facing immutable proposal-snapshots.
/// </summary>
public interface IBuyerProposalRevisionSnapshotService
{
    /// <summary>
    /// Bygger canonical buyer-facing JSON och SHA-256 för revisionen.
    /// </summary>
    Task<CanonicalJsonPayload> CreateAsync(Guid buyerProposalRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Beräknar SHA-256 från ett redan lagrat snapshot.
    /// </summary>
    string ComputeHash(string snapshotJson);
}