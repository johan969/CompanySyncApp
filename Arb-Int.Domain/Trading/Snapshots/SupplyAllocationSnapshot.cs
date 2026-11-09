using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller en fryst supplier-allokering.
/// </summary>
public sealed class SupplyAllocationSnapshot
{
    /// <summary>
    /// Anger allocationens ordning inom solution line.
    /// </summary>
    public int AllocationSequence { get; init; }

    /// <summary>
    /// Identifierar supplier-relationen.
    /// </summary>
    public Guid SupplierBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar supplier-erbjudandet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar eventuell availability-post.
    /// </summary>
    public Guid? SupplierOfferingAvailabilityId { get; init; }

    /// <summary>
    /// Identifierar eventuell inventory lot.
    /// </summary>
    public Guid? InventoryLotId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier quote line.
    /// </summary>
    public Guid? SupplierQuoteLineId { get; init; }

    /// <summary>
    /// Anger allocationens status.
    /// </summary>
    public SupplyAllocationStatus Status { get; init; }

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
    /// Anger uppskattad ytterligare kostnad.
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
    /// Innehåller interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }
}