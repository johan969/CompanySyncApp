using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar samtliga konkreta requirement-typer inom draft-revisioner.
/// </summary>
public interface IBuyerRfqRequirementService
{
    /// <summary>
    /// Hämtar krav för en RFQ-revision.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerRfqRequirementResponse>>> GetByRevisionAsync(Guid buyerRfqRevisionId, bool includeInactive = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar krav som gäller en specifik RFQ-rad.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerRfqRequirementResponse>>> GetByLineAsync(Guid buyerRfqLineId, bool includeInactive = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar ett enskilt RFQ-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> GetByIdAsync(Guid buyerRfqRequirementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett materialkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateMaterialAsync(Guid buyerRfqRevisionId, CreateBuyerRfqMaterialRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett materialkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateMaterialAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqMaterialRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett biological-source-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateBiologicalSourceAsync(Guid buyerRfqRevisionId, CreateBuyerRfqBiologicalSourceRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett biological-source-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateBiologicalSourceAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqBiologicalSourceRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett ursprungskrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateOriginAsync(Guid buyerRfqRevisionId, CreateBuyerRfqOriginRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett ursprungskrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateOriginAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqOriginRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett certifieringskrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateCertificationAsync(Guid buyerRfqRevisionId, CreateBuyerRfqCertificationRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett certifieringskrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateCertificationAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqCertificationRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett dokumentkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateDocumentAsync(Guid buyerRfqRevisionId, CreateBuyerRfqDocumentRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett dokumentkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateDocumentAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqDocumentRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett constituent-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateConstituentAsync(Guid buyerRfqRevisionId, CreateBuyerRfqConstituentRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett constituent-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateConstituentAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqConstituentRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett characteristic-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateCharacteristicAsync(Guid buyerRfqRevisionId, CreateBuyerRfqCharacteristicRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett characteristic-krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateCharacteristicAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqCharacteristicRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett processkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateProcessAsync(Guid buyerRfqRevisionId, CreateBuyerRfqProcessRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett processkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateProcessAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqProcessRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett kommersiellt krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateCommercialAsync(Guid buyerRfqRevisionId, CreateBuyerRfqCommercialRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett kommersiellt krav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateCommercialAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqCommercialRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett logistikkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> CreateLogisticsAsync(Guid buyerRfqRevisionId, CreateBuyerRfqLogisticsRequirementRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett logistikkrav.
    /// </summary>
    Task<Result<BuyerRfqRequirementResponse>> UpdateLogisticsAsync(Guid buyerRfqRequirementId, UpdateBuyerRfqLogisticsRequirementRequest request, CancellationToken cancellationToken = default);
}