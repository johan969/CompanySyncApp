using Arb_Int.Domain.Services.Common;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Bygger och verifierar immutable snapshots för supplier-quote-revisioner.
/// </summary>
public interface ISupplierQuoteRevisionSnapshotService
{
    /// <summary>
    /// Bygger canonical JSON och SHA-256 från revisionens kommersiella innehåll.
    /// </summary>
    Task<CanonicalJsonPayload> CreateAsync(Guid supplierQuoteRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Beräknar SHA-256 från canonical representation av ett lagrat snapshot.
    /// </summary>
    string ComputeHash(string snapshotJson);
}