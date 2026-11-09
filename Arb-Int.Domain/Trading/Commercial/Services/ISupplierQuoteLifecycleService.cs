using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar review, intern acceptans, rejection, withdrawal och expiration för immutable supplier quotes.
/// </summary>
public interface ISupplierQuoteLifecycleService
{
    /// <summary>
    /// Kontrollerar om en mottagen revision är redo för intern review.
    /// </summary>
    Task<Result<SupplierQuoteReviewReadinessResponse>> GetReviewReadinessAsync(Guid supplierQuoteId, Guid supplierQuoteRevisionId, DateTimeOffset evaluatedAtUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flyttar en Received quote till UnderReview utan att ändra revisionens immutable snapshot.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> BeginReviewAsync(Guid supplierQuoteId, BeginSupplierQuoteReviewRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar aktuell reviewed revision som AcceptedInternally.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> AcceptInternallyAsync(Guid supplierQuoteId, AcceptSupplierQuoteInternallyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar aktuell reviewed revision som Rejected.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> RejectAsync(Guid supplierQuoteId, RejectSupplierQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar hela supplier-quoten som Withdrawn utan att skriva om den immutable revisionens historik.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> WithdrawAsync(Guid supplierQuoteId, WithdrawSupplierQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Markerar en due current revision och dess quote-root som Expired.
    /// </summary>
    Task<Result<SupplierQuoteResponse>> ExpireAsync(Guid supplierQuoteId, ExpireSupplierQuoteRequest request, DateTimeOffset evaluatedAtUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Expire:ar en begränsad batch av current supplier quotes vars ValidToUtc har passerat.
    /// </summary>
    Task<Result<SupplierQuoteExpirationBatchResponse>> ExpireDueAsync(DateTimeOffset evaluatedAtUtc, int batchSize = 500, CancellationToken cancellationToken = default);
}