namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller gemensamma värden för alla konkreta RFQ-krav.
/// </summary>
public abstract class BuyerRfqRequirementRequestBase
{
    /// <summary>
    /// Identifierar eventuell RFQ-rad som kravet ska gälla. Null innebär revisionsnivå.
    /// </summary>
    public Guid? BuyerRfqLineId { get; init; }

    /// <summary>
    /// Kravets namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Kravets beskrivning.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Anger om kravet är obligatoriskt, föredraget eller informativt.
    /// </summary>
    public RequirementLevel Level { get; init; }

    /// <summary>
    /// Anger kravets vikt vid utvärdering.
    /// </summary>
    public decimal Weight { get; init; }

    /// <summary>
    /// Grupperar alternativa krav som hör samman.
    /// </summary>
    public string? AlternativeGroupKey { get; init; }

    /// <summary>
    /// Anger kravets sorteringsordning.
    /// </summary>
    public int SortOrder { get; init; }

    /// <summary>
    /// Köparens ursprungliga formulering.
    /// </summary>
    public string? BuyerOriginalText { get; init; }

    /// <summary>
    /// Interna anteckningar om kravet.
    /// </summary>
    public string? InternalNotes { get; init; }
}