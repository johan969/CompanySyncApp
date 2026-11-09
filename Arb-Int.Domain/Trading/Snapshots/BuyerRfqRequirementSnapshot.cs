using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading.Snapshots;

/// <summary>
/// Innehåller gemensamma och typspecifika värden för ett requirement i snapshotet.
/// </summary>
public sealed class BuyerRfqRequirementSnapshot
{
    /// <summary>
    /// Anger den konkreta requirement-typen.
    /// </summary>
    public BuyerRfqRequirementSnapshotType RequirementType { get; set; }

    /// <summary>
    /// Anger radnummer eller null när kravet gäller hela revisionen.
    /// </summary>
    public int? BuyerRfqLineNumber { get; set; }

    /// <summary>
    /// Kravets namn.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Kravets beskrivning.
    /// </summary>
    public required string Description { get; set; }

    /// <summary>
    /// Kravets nivå.
    /// </summary>
    public RequirementLevel Level { get; set; }

    /// <summary>
    /// Kravets vikt.
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// Grupperar alternativa krav.
    /// </summary>
    public string? AlternativeGroupKey { get; set; }

    /// <summary>
    /// Anger om kravet används vid matching.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Kravets sorteringsordning.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Köparens ursprungliga formulering.
    /// </summary>
    public string? BuyerOriginalText { get; set; }

    /// <summary>
    /// Interna anteckningar.
    /// </summary>
    public string? InternalNotes { get; set; }

    /// <summary>
    /// Identifierar eventuellt material.
    /// </summary>
    public Guid? MaterialId { get; set; }

    /// <summary>
    /// Identifierar eventuell materialtyp.
    /// </summary>
    public int? MaterialTypeId { get; set; }

    /// <summary>
    /// Anger eventuell materialmatchningsmetod.
    /// </summary>
    public MaterialMatchMode? MaterialMatchMode { get; set; }

    /// <summary>
    /// Anger om substitut är tillåtna för ett materialkrav.
    /// </summary>
    public bool? AllowSubstitutes { get; set; }

    /// <summary>
    /// Identifierar eventuell biologisk källa.
    /// </summary>
    public Guid? BiologicalSourceId { get; set; }

    /// <summary>
    /// Anger eventuell requirement-operator.
    /// </summary>
    public RequirementOperator? Operator { get; set; }

    /// <summary>
    /// Identifierar eventuell geografisk plats.
    /// </summary>
    public Guid? GeographicLocationId { get; set; }

    /// <summary>
    /// Identifierar eventuellt land.
    /// </summary>
    public Guid? CountryId { get; set; }

    /// <summary>
    /// Anger eventuell ursprungsroll.
    /// </summary>
    public OriginRole? OriginRole { get; set; }

    /// <summary>
    /// Identifierar eventuellt certifieringssystem.
    /// </summary>
    public int? CertificationSchemeId { get; set; }

    /// <summary>
    /// Anger eventuellt certifieringsscope.
    /// </summary>
    public CertificationScope? CertificationScope { get; set; }

    /// <summary>
    /// Identifierar eventuell certifieringsjurisdiktion.
    /// </summary>
    public int? CertificationSchemeJurisdictionId { get; set; }

    /// <summary>
    /// Anger om certifieringen måste vara giltig vid leverans.
    /// </summary>
    public bool? MustBeValidAtDelivery { get; set; }

    /// <summary>
    /// Identifierar eventuell dokumenttyp.
    /// </summary>
    public int? DocumentTypeId { get; set; }

    /// <summary>
    /// Anger om dokumentet måste vara lot-specifikt.
    /// </summary>
    public bool? MustBeLotSpecific { get; set; }

    /// <summary>
    /// Anger om dokumentet måste utfärdas av ackrediterad part.
    /// </summary>
    public bool? MustBeIssuedByAccreditedParty { get; set; }

    /// <summary>
    /// Anger maximal dokumentålder.
    /// </summary>
    public int? MaximumAgeDays { get; set; }

    /// <summary>
    /// Identifierar eventuell constituent.
    /// </summary>
    public Guid? ConstituentId { get; set; }

    /// <summary>
    /// Anger eventuellt minimum.
    /// </summary>
    public decimal? MinimumValue { get; set; }

    /// <summary>
    /// Anger eventuellt maximum.
    /// </summary>
    public decimal? MaximumValue { get; set; }

    /// <summary>
    /// Anger eventuellt målvärde.
    /// </summary>
    public decimal? TargetValue { get; set; }

    /// <summary>
    /// Identifierar eventuell mätenhet.
    /// </summary>
    public int? UnitOfMeasureId { get; set; }

    /// <summary>
    /// Anger eventuell measurement basis.
    /// </summary>
    public MeasurementBasis? MeasurementBasis { get; set; }

    /// <summary>
    /// Anger eventuell analysmetod.
    /// </summary>
    public string? AnalysisMethod { get; set; }

    /// <summary>
    /// Anger om constituent måste verifieras för varje lot.
    /// </summary>
    public bool? MustBeVerifiedForEveryLot { get; set; }

    /// <summary>
    /// Anger om ackrediterat laboratorium krävs.
    /// </summary>
    public bool? AccreditedLaboratoryRequired { get; set; }

    /// <summary>
    /// Identifierar eventuell characteristic.
    /// </summary>
    public Guid? CharacteristicId { get; set; }

    /// <summary>
    /// Anger eventuellt förväntat textvärde.
    /// </summary>
    public string? ExpectedTextValue { get; set; }

    /// <summary>
    /// Anger eventuellt förväntat numeriskt värde.
    /// </summary>
    public decimal? ExpectedNumericValue { get; set; }

    /// <summary>
    /// Identifierar eventuell processtyp.
    /// </summary>
    public int? ProcessTypeId { get; set; }

    /// <summary>
    /// Anger eventuell process-mode.
    /// </summary>
    public ProcessRequirementMode? ProcessMode { get; set; }

    /// <summary>
    /// Innehåller eventuella processparametrar.
    /// </summary>
    public string? ProcessParametersJson { get; set; }

    /// <summary>
    /// Anger eventuell kommersiell requirement-typ.
    /// </summary>
    public CommercialRequirementKind? CommercialKind { get; set; }

    /// <summary>
    /// Anger eventuell logistik-requirement-typ.
    /// </summary>
    public LogisticsRequirementKind? LogisticsKind { get; set; }

    /// <summary>
    /// Anger eventuellt generiskt numeriskt värde.
    /// </summary>
    public decimal? NumericValue { get; set; }

    /// <summary>
    /// Anger eventuellt generiskt textvärde.
    /// </summary>
    public string? TextValue { get; set; }

    /// <summary>
    /// Identifierar eventuell valuta.
    /// </summary>
    public int? CurrencyId { get; set; }

    /// <summary>
    /// Identifierar eventuell Incoterm-regel.
    /// </summary>
    public int? IncotermRuleId { get; set; }
}