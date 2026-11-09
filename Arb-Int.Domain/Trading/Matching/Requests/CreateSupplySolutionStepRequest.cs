namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller data för att skapa ett steg i en genererad supply solution.
/// </summary>
public sealed class CreateSupplySolutionStepRequest
{
    /// <summary>
    /// Identifierar eventuell solution line som steget hör till.
    /// </summary>
    public Guid? SupplySolutionLineId { get; init; }

    /// <summary>
    /// Anger stegets sekvensnummer inom kandidaten.
    /// </summary>
    public int SequenceNumber { get; init; }

    /// <summary>
    /// Anger vilken typ av supply-, process- eller logistiksteg som representeras.
    /// </summary>
    public SupplySolutionStepType Type { get; init; }

    /// <summary>
    /// Stegets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver stegets funktion i lösningen.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Identifierar eventuell business relationship som utför steget.
    /// </summary>
    public Guid? ProviderBusinessRelationshipId { get; init; }

    /// <summary>
    /// Identifierar eventuell supplier process capability som stödjer steget.
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
    /// Identifierar enheten för förväntad inputkvantitet.
    /// </summary>
    public int? InputQuantityUnitId { get; init; }

    /// <summary>
    /// Anger förväntad outputkvantitet.
    /// </summary>
    public decimal? ExpectedOutputQuantity { get; init; }

    /// <summary>
    /// Identifierar enheten för förväntad outputkvantitet.
    /// </summary>
    public int? OutputQuantityUnitId { get; init; }

    /// <summary>
    /// Anger förväntad yield i procent.
    /// </summary>
    public decimal? ExpectedYieldPercent { get; init; }

    /// <summary>
    /// Anger uppskattad ledtid för steget i dagar.
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
    /// Innehåller typspecifika parametrar för steget.
    /// </summary>
    public string? ParametersJson { get; init; }
}