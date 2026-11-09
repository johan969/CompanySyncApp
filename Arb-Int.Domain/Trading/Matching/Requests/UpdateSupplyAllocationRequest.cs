using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller redigerbara värden för en befintlig supplier-allokering.
/// </summary>
public sealed class UpdateSupplyAllocationRequest
{
    /// <summary>
    /// Identifierar eventuell tillgänglighetspost för samma offering.
    /// </summary>
    public Guid? SupplierOfferingAvailabilityId { get; init; }

    /// <summary>
    /// Identifierar eventuell lagerlot från samma supplier.
    /// </summary>
    public Guid? InventoryLotId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier-quote-line.
    /// </summary>
    public Guid? SupplierQuoteLineId { get; init; }

    /// <summary>
    /// Anger allokeringens status.
    /// </summary>
    public SupplyAllocationStatus Status { get; init; }

    /// <summary>
    /// Anger allokeringens ordning inom solution line.
    /// </summary>
    public int AllocationSequence { get; init; }

    /// <summary>
    /// Anger allokerad kvantitet.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger eventuellt inköpspris per enhet.
    /// </summary>
    public decimal? UnitPurchasePrice { get; init; }

    /// <summary>
    /// Identifierar inköpsprisets valuta.
    /// </summary>
    public int? PurchasePriceCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad ytterligare kostnad.
    /// </summary>
    public decimal? EstimatedAdditionalCost { get; init; }

    /// <summary>
    /// Anger availability-verifieringen.
    /// </summary>
    public VerificationState AvailabilityVerificationState { get; init; }

    /// <summary>
    /// Anger kvalitetsverifieringen.
    /// </summary>
    public VerificationState QualityVerificationState { get; init; }

    /// <summary>
    /// Interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från senast lästa allocation.
    /// </summary>
    public uint RowVersion { get; init; }
}