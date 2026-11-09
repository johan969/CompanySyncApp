namespace Arb_Int.Domain.Trading;

/// <summary>
/// Innehåller ett krav från en RFQ-revision med typspecifika detaljer.
/// </summary>
public sealed class BuyerRfqRequirementResponse
{
    /// <summary>
    /// Identifierar kravet.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar revisionen som kravet tillhör.
    /// </summary>
    public Guid BuyerRfqRevisionId { get; init; }

    /// <summary>
    /// Identifierar eventuell RFQ-rad som kravet gäller.
    /// </summary>
    public Guid? BuyerRfqLineId { get; init; }

    /// <summary>
    /// Radnummer när kravet gäller en specifik RFQ-rad.
    /// </summary>
    public int? BuyerRfqLineNumber { get; init; }

    /// <summary>
    /// Anger kravets konkreta typ.
    /// </summary>
    public BuyerRfqRequirementType RequirementType { get; init; }

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
    /// Anger om kravet ska användas vid utvärdering.
    /// </summary>
    public bool IsActive { get; init; }

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

    /// <summary>
    /// Typspecifika värden för ett materialkrav.
    /// </summary>
    public BuyerRfqMaterialRequirementDetailsResponse? Material { get; init; }

    /// <summary>
    /// Typspecifika värden för ett biologiskt source-krav.
    /// </summary>
    public BuyerRfqBiologicalSourceRequirementDetailsResponse? BiologicalSource { get; init; }

    /// <summary>
    /// Typspecifika värden för ett ursprungskrav.
    /// </summary>
    public BuyerRfqOriginRequirementDetailsResponse? Origin { get; init; }

    /// <summary>
    /// Typspecifika värden för ett certifieringskrav.
    /// </summary>
    public BuyerRfqCertificationRequirementDetailsResponse? Certification { get; init; }

    /// <summary>
    /// Typspecifika värden för ett dokumentkrav.
    /// </summary>
    public BuyerRfqDocumentRequirementDetailsResponse? Document { get; init; }

    /// <summary>
    /// Typspecifika värden för ett constituent-krav.
    /// </summary>
    public BuyerRfqConstituentRequirementDetailsResponse? Constituent { get; init; }

    /// <summary>
    /// Typspecifika värden för ett characteristic-krav.
    /// </summary>
    public BuyerRfqCharacteristicRequirementDetailsResponse? Characteristic { get; init; }

    /// <summary>
    /// Typspecifika värden för ett processkrav.
    /// </summary>
    public BuyerRfqProcessRequirementDetailsResponse? Process { get; init; }

    /// <summary>
    /// Typspecifika värden för ett kommersiellt krav.
    /// </summary>
    public BuyerRfqCommercialRequirementDetailsResponse? Commercial { get; init; }

    /// <summary>
    /// Typspecifika värden för ett logistikkrav.
    /// </summary>
    public BuyerRfqLogisticsRequirementDetailsResponse? Logistics { get; init; }

    /// <summary>
    /// Tidpunkt då kravet skapades.
    /// </summary>
    public DateTimeOffset? CreateDateUtc { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}