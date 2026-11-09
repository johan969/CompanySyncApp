using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Validerar RFQ-, candidate-, disclosure- och line-provenance för buyer proposals.
/// </summary>
public interface IBuyerProposalReferenceValidator
{
    /// <summary>
    /// Kontrollerar att RFQ-revisionen tillhör proposalets RFQ och är ReleasedForMatching.
    /// </summary>
    Task<Result> ValidateBuyerRfqRevisionAsync(Guid buyerRfqId, Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar att proposal-valutan existerar.
    /// </summary>
    Task<Result> ValidateCurrencyAsync(int currencyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar disclosure-revisionens buyer-scope utan att kräva att den redan är Approved.
    /// </summary>
    Task<Result> ValidateDisclosureReferenceAsync(Guid buyerBusinessRelationshipId, Guid? disclosurePackageRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar att en candidate kan ligga bakom en Draft proposal-option.
    /// </summary>
    Task<Result> ValidateCandidateAsync(Guid buyerRfqRevisionId, Guid supplySolutionCandidateId, DateTimeOffset evaluatedAtUtc, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar RFQ-line och eventuell SupplySolutionLine samt övriga line-referenser.
    /// </summary>
    Task<Result> ValidateLineAsync(Guid buyerRfqRevisionId, Guid supplySolutionCandidateId, Guid buyerRfqLineId, Guid? supplySolutionLineId, Guid? materialId, Guid? productId, int quantityUnitId, CancellationToken cancellationToken = default);
}