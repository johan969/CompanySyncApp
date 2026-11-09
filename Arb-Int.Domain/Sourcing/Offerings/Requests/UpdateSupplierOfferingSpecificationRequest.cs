namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbar data för en supplier offering specification.
    /// </summary>
    public sealed class UpdateSupplierOfferingSpecificationRequest
    {
        /// <summary>
        /// Anger vilken typ av specification posten representerar.
        /// </summary>
        public SupplierOfferingSpecificationKind Kind { get; init; }

        /// <summary>
        /// Anger specificationens visningsnamn.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Beskriver specificationen.
        /// </summary>
        public required string Description { get; init; }

        /// <summary>
        /// Identifierar constituent när Kind är Constituent.
        /// </summary>
        public Guid? ConstituentId { get; init; }

        /// <summary>
        /// Identifierar characteristic när Kind är Characteristic.
        /// </summary>
        public Guid? CharacteristicId { get; init; }

        /// <summary>
        /// Anger hur specificationens värde ska tolkas.
        /// </summary>
        public RequirementOperator Operator { get; init; }

        /// <summary>
        /// Anger lägsta numeriska värde när det är relevant.
        /// </summary>
        public decimal? MinimumValue { get; init; }

        /// <summary>
        /// Anger högsta numeriska värde när det är relevant.
        /// </summary>
        public decimal? MaximumValue { get; init; }

        /// <summary>
        /// Anger målvärde när det är relevant.
        /// </summary>
        public decimal? TargetValue { get; init; }

        /// <summary>
        /// Anger textvärde när specificationen uttrycks som text.
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
        /// Anger analysmetod när en sådan har deklarerats.
        /// </summary>
        public string? AnalysisMethod { get; init; }

        /// <summary>
        /// Anger specificationens verifieringsstatus.
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
        /// Identifierar ett supporting supply document när ett sådant finns.
        /// </summary>
        public Guid? SupplyDocumentId { get; init; }

        /// <summary>
        /// Concurrency-token för specification-raden.
        /// </summary>
        public uint RowVersion { get; init; }

        /// <summary>
        /// Concurrency-token för offering-aggregatet.
        /// </summary>
        public uint SupplierMaterialOfferingRowVersion { get; init; }
    }
}
