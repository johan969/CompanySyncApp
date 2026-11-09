using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller en konkret supplier-allokering för en solution line.
/// </summary>
public sealed class SupplyAllocationResponse
{
    /// <summary>
    /// Identifierar allokeringen.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar solution line.
    /// </summary>
    public Guid SupplySolutionLineId { get; init; }

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
    /// Anger allokeringens status.
    /// </summary>
    public SupplyAllocationStatus Status { get; init; }

    /// <summary>
    /// Anger allokeringens ordning.
    /// </summary>
    public int AllocationSequence { get; init; }

    /// <summary>
    /// Allokerad kvantitet.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Identifierar kvantitetsenheten.
    /// </summary>
    public int QuantityUnitId { get; init; }

    /// <summary>
    /// Eventuellt inköpspris per enhet.
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
    /// Availability-informationens verifieringsstatus.
    /// </summary>
    public VerificationState AvailabilityVerificationState { get; init; }

    /// <summary>
    /// Kvalitetsinformationens verifieringsstatus.
    /// </summary>
    public VerificationState QualityVerificationState { get; init; }

    /// <summary>
    /// Interna anteckningar.
    /// </summary>
    public string? Notes { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}