using FluentResults;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Hanterar leveranstillfällen och line-allokeringar inom RFQ-revisioner.
/// </summary>
public interface IBuyerRfqDeliveryService
{
    /// <summary>
    /// Hämtar samtliga leveranstillfällen för en revision.
    /// </summary>
    Task<Result<IReadOnlyList<BuyerRfqDeliveryResponse>>> GetByRevisionAsync(Guid buyerRfqRevisionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hämtar ett enskilt leveranstillfälle.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> GetByIdAsync(Guid buyerRfqDeliveryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Skapar ett leveranstillfälle i en draft-revision.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> CreateAsync(Guid buyerRfqRevisionId, CreateBuyerRfqDeliveryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar ett leveranstillfälle i en draft-revision.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> UpdateAsync(Guid buyerRfqDeliveryId, UpdateBuyerRfqDeliveryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort ett leveranstillfälle och dess line-allokeringar från en draft-revision.
    /// </summary>
    Task<Result> DeleteAsync(Guid buyerRfqDeliveryId, uint rowVersion, CancellationToken cancellationToken = default);

    /// <summary>
    /// Allokerar en RFQ-rad till ett leveranstillfälle.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> AddLineAllocationAsync(Guid buyerRfqDeliveryId, CreateBuyerRfqDeliveryLineAllocationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Uppdaterar kvantiteten för en befintlig delivery-line-allokering.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> UpdateLineAllocationAsync(Guid buyerRfqDeliveryLineAllocationId, UpdateBuyerRfqDeliveryLineAllocationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Tar bort en delivery-line-allokering.
    /// </summary>
    Task<Result<BuyerRfqDeliveryResponse>> DeleteLineAllocationAsync(Guid buyerRfqDeliveryLineAllocationId, uint rowVersion, CancellationToken cancellationToken = default);
}