using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för en konkret supplier-allokering bakom en solution line.
/// </summary>
public sealed class CreateSupplyAllocationRequest
{
    /// <summary>
    /// Identifierar supplier-relationen som används.
    /// </summary>
    public Guid SupplierBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar erbjudandet som levererar materialet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Identifierar eventuell tillgänglighetspost för erbjudandet.
    /// </summary>
    public Guid? SupplierOfferingAvailabilityId { get; init; }

    /// <summary>
    /// Identifierar eventuell konkret lagerlot.
    /// </summary>
    public Guid? InventoryLotId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier-quote-line som pris eller tillgänglighet kommer från.
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
    /// Identifierar enheten för den allokerade kvantiteten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Anger eventuellt inköpspris per enhet.
    /// </summary>
    public decimal? UnitPurchasePrice { get; init; }

    /// <summary>
    /// Identifierar valutan för UnitPurchasePrice.
    /// </summary>
    public int? PurchasePriceCurrencyId { get; init; }

    /// <summary>
    /// Uppskattad ytterligare kostnad för allokeringen.
    /// </summary>
    public decimal? EstimatedAdditionalCost { get; init; }

    /// <summary>
    /// Anger hur väl availability-informationen är verifierad.
    /// </summary>
    public VerificationState AvailabilityVerificationState { get; init; }

    /// <summary>
    /// Anger hur väl kvalitetsinformationen är verifierad.
    /// </summary>
    public VerificationState QualityVerificationState { get; init; }

    /// <summary>
    /// Interna anteckningar om allokeringen.
    /// </summary>
    public string? Notes { get; init; }
}