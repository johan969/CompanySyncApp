using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar BuyerRfq-roten, konvertering från BuyerDemand och RFQ-revisionernas livscykel.
/// </summary>
public interface IBuyerRfqService
{
    /// <summary>
    /// Hämtar en BuyerRfq med revisionsmetadata.
    /// </summary>
    Task<Result<BuyerRfqResponse>> GetByIdAsync(Guid buyerRfqId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar en enskild RFQ-revision.
    /// </summary>
    Task<Result<BuyerRfqRevisionResponse>> GetRevisionByIdAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Söker BuyerRfq för en intern organisation.
    /// </summary>
    Task<Result<PagedResult<BuyerRfqListResponse>>> QueryAsync(Guid owningOrganizationId, BuyerRfqQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar en draft-RFQ med draft-revision 1 på en buyer-relation.
    /// </summary>
    Task<Result<BuyerRfqResponse>> CreateAsync(Guid buyerBusinessRelationshipId, CreateBuyerRfqRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Konverterar en BuyerDemand till en draft-RFQ och markerar demanden som konverterad.
    /// </summary>
    Task<Result<BuyerRfqResponse>> CreateFromDemandAsync(Guid buyerDemandId, CreateBuyerRfqFromDemandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar RFQ-rotens metadata med optimistic concurrency.
    /// </summary>
    Task<Result<BuyerRfqResponse>> UpdateAsync(Guid buyerRfqId, UpdateBuyerRfqRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar metadata på en draft-revision.
    /// </summary>
    Task<Result<BuyerRfqRevisionResponse>> UpdateRevisionAsync(Guid buyerRfqRevisionId, UpdateBuyerRfqRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ändrar RFQ:ns root-status enligt tillåtna livscykelövergångar.
    /// </summary>
    Task<Result<BuyerRfqResponse>> ChangeStatusAsync(Guid buyerRfqId, ChangeBuyerRfqStatusRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Avbryter en draft-revision som inte längre ska användas.
    /// </summary>
    Task<Result<BuyerRfqRevisionResponse>> CancelRevisionAsync(Guid buyerRfqRevisionId, CancelBuyerRfqRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Kontrollerar om en draft-revision är komplett och konsekvent nog för release.
    /// </summary>
    Task<Result<BuyerRfqRevisionReleaseReadinessResponse>> GetReleaseReadinessAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Låser en draft-revision, skapar snapshot och gör den till aktuell matching-revision.
    /// </summary>
    Task<Result<BuyerRfqResponse>> ReleaseRevisionAsync(Guid buyerRfqRevisionId, ReleaseBuyerRfqRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deep-clonar aktuell released revision till nästa draft-revision.
    /// </summary>
    Task<Result<BuyerRfqResponse>> CreateNextRevisionAsync(Guid buyerRfqId, CreateNextBuyerRfqRevisionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifierar integriteten för ett sparat revisionssnapshot.
    /// </summary>
    Task<Result<BuyerRfqSnapshotVerificationResponse>> VerifyRevisionSnapshotAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);
}