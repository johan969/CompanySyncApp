using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar supplier-quote-roten, revisioner och immutable received snapshots.
/// </summary>
public interface ISupplierQuoteService
{
    /// <summary>
    /// Hämtar en supplier quote med hela revisionshistoriken.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> GetByIdAsync(Guid supplierQuoteId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker supplier quotes för en intern organisation.
    /// </summary>
    Task<Result<PagedResult<SupplierQuoteListResponse>>> QueryAsync(Guid owningOrganizationId, SupplierQuoteQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en specifik supplier-quote-revision.
    /// </summary>
    Task<Result<SupplierQuoteRevisionResponse>> GetRevisionByIdAsync(Guid supplierQuoteRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar supplier-quote-roten tillsammans med revision 1 som Draft.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> CreateAsync(Guid supplierBusinessRelationshipId, CreateSupplierQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar metadata på quote-roten.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> UpdateAsync(Guid supplierQuoteId, UpdateSupplierQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar kommersiella villkor på en Draft-revision.
    /// </summary>
    Task<Result<SupplierQuoteRevisionResponse>> UpdateRevisionAsync(Guid supplierQuoteRevisionId, UpdateSupplierQuoteRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Klonar senaste frysta revision till nästa Draft-revision.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> CreateNextRevisionAsync(Guid supplierQuoteId, CreateNextSupplierQuoteRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar om revisionen kan tas emot och frysas.
    /// </summary>
    Task<Result<SupplierQuoteRevisionReceiveReadinessResponse>> GetReceiveReadinessAsync(Guid supplierQuoteRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Fryser en Draft-revision och markerar den som Received.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> ReceiveRevisionAsync(Guid supplierQuoteRevisionId, ReceiveSupplierQuoteRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifierar att ett lagrat revision-snapshot fortfarande matchar sin hash.
    /// </summary>
    Task<Result<SupplierQuoteRevisionSnapshotVerificationResponse>> VerifyRevisionSnapshotAsync(Guid supplierQuoteRevisionId, CancellationToken cancellationToken = default);
}