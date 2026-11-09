using System.Text.Json;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller ett fryst supply-, process- eller logistiksteg.
/// </summary>
public sealed class SupplySolutionStepSnapshot
{
    /// <summary>
    /// Anger stegets stabila sekvensnummer inom kandidaten.
    /// </summary>
    public int SequenceNumber { get; init; }

    /// <summary>
    /// Anger eventuell RFQ-rad som steget hör till.
    /// </summary>
    public int? BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Anger stegets typ.
    /// </summary>
    public SupplySolutionStepType Type { get; init; }

    /// <summary>
    /// Anger stegets maskinella status.
    /// </summary>
    public SupplySolutionStepStatus Status { get; init; }

    /// <summary>
    /// Stegets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Stegets beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Identifierar eventuell provider.
    /// </summary>
    public Guid? ProviderBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar eventuell process capability.
    /// </summary>
    public Guid? SupplierProcessCapabilityId { get; init; }

    /// <summary>
    /// Identifierar eventuell processtyp.
    /// </summary>
    public int? ProcessTypeId { get; init; }

    /// <summary>
    /// Identifierar eventuellt inputmaterial.
    /// </summary>
    public Guid? InputMaterialId { get; init; }

    /// <summary>
    /// Identifierar eventuellt outputmaterial.
    /// </summary>
    public Guid? OutputMaterialId { get; init; }

    /// <summary>
    /// Anger förväntad inputkvantitet.
    /// </summary>
    public decimal? ExpectedInputQuantity { get; init; }

    /// <summary>
    /// Identifierar inputenheten.
    /// </summary>
    public int? InputQuantityUnitId { get; init; }

    /// <summary>
    /// Anger förväntad outputkvantitet.
    /// </summary>
    public decimal? ExpectedOutputQuantity { get; init; }

    /// <summary>
    /// Identifierar outputenheten.
    /// </summary>
    public int? OutputQuantityUnitId { get; init; }

    /// <summary>
    /// Anger förväntad yield.
    /// </summary>
    public decimal? ExpectedYieldPercent { get; init; }

    /// <summary>
    /// Anger uppskattad ledtid.
    /// </summary>
    public int? EstimatedLeadTimeDays { get; init; }

    /// <summary>
    /// Identifierar eventuell startplats.
    /// </summary>
    public Guid? FromLocationId { get; init; }

    /// <summary>
    /// Identifierar eventuell slutplats.
    /// </summary>
    public Guid? ToLocationId { get; init; }

    /// <summary>
    /// Anger planerad starttid.
    /// </summary>
    public DateTimeOffset? PlannedStartUtc { get; init; }

    /// <summary>
    /// Anger planerad sluttid.
    /// </summary>
    public DateTimeOffset? PlannedEndUtc { get; init; }

    /// <summary>
    /// Innehåller typspecifika parametrar.
    /// </summary>
    public JsonElement? Parameters { get; init; }
}