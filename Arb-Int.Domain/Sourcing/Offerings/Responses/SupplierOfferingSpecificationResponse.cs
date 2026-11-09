namespace Arb_Int.Domain.Sourcing;

/// <summary>
/// Innehåller en mätbar eller semantisk specification för ett supplier offering.
/// </summary>
public sealed class SupplierOfferingSpecificationResponse
{
    /// <summary>
    /// Identifierar specification-posten.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identifierar offeringet.
    /// </summary>
    public Guid SupplierMaterialOfferingId { get; init; }

    /// <summary>
    /// Anger specificationens typ.
    /// </summary>
    public SupplierOfferingSpecificationKind Kind { get; init; }

    /// <summary>
    /// Anger specificationens namn.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Beskriver specificationen.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Identifierar constituent när specificationen gäller en constituent.
    /// </summary>
    public Guid? ConstituentId { get; init; }

    /// <summary>
    /// Identifierar characteristic när specificationen gäller en characteristic.
    /// </summary>
    public Guid? CharacteristicId { get; init; }

    /// <summary>
    /// Anger operatorn för specificationens värde.
    /// </summary>
    public RequirementOperator Operator { get; init; }

    /// <summary>
    /// Anger lägsta numeriska värde när relevant.
    /// </summary>
    public decimal? MinimumValue { get; init; }

    /// <summary>
    /// Anger högsta numeriska värde när relevant.
    /// </summary>
    public decimal? MaximumValue { get; init; }

    /// <summary>
    /// Anger målvärde när relevant.
    /// </summary>
    public decimal? TargetValue { get; init; }

    /// <summary>
    /// Anger textvärde när specificationen inte är numerisk.
    /// </summary>
    public string? TextValue { get; init; }

    /// <summary>
    /// Identifierar måttenheten när specificationen använder en sådan.
    /// </summary>
    public int? UnitOfMeasureId { get; init; }

    /// <summary>
    /// Anger measurement basis för värdet.
    /// </summary>
    public MeasurementBasis Basis { get; init; }

    /// <summary>
    /// Anger analysmetod när en sådan deklarerats.
    /// </summary>
    public string? AnalysisMethod { get; init; }

    /// <summary>
    /// Anger aktuell verifieringsstatus för specificationen.
    /// </summary>
    public VerificationState VerificationState { get; init; }

    /// <summary>
    /// Anger när specificationen börjar gälla.
    /// </summary>
    public DateTimeOffset? ValidFromUtc { get; init; }

    /// <summary>
    /// Anger när specificationen slutar gälla.
    /// </summary>
    public DateTimeOffset? ValidToUtc { get; init; }

    /// <summary>
    /// Identifierar dokumentet som stöder specificationen när ett sådant finns.
    /// </summary>
    public Guid? SupplyDocumentId { get; init; }

    /// <summary>
    /// Concurrency-token från databasen.
    /// </summary>
    public uint RowVersion { get; init; }
}
