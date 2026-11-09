namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller ett mät- eller textresultat från en supply analysis.
    /// </summary>
    public sealed class SupplyAnalysisResultResponse
    {
        /// <summary>
        /// Identifierar resultatet.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Identifierar analysen som äger resultatet.
        /// </summary>
        public Guid SupplyAnalysisId { get; init; }

        /// <summary>
        /// Identifierar canonical constituent när resultatet är mappat till en sådan.
        /// </summary>
        public Guid? ConstituentId { get; init; }

        /// <summary>
        /// Identifierar canonical characteristic när resultatet är mappat till en sådan.
        /// </summary>
        public Guid? CharacteristicId { get; init; }

        /// <summary>
        /// Bevarar rått analyte-namn när källan använder ett eget namn.
        /// </summary>
        public string? AnalyteName { get; init; }

        /// <summary>
        /// Anger hur det rapporterade värdet ska tolkas.
        /// </summary>
        public AnalysisResultQualifier Qualifier { get; init; }

        /// <summary>
        /// Anger numeriskt värde när resultatet är numeriskt.
        /// </summary>
        public decimal? NumericValue { get; init; }

        /// <summary>
        /// Anger övre gräns när resultatet är ett intervall.
        /// </summary>
        public decimal? RangeMaximumValue { get; init; }

        /// <summary>
        /// Anger textvärde när resultatet inte uttrycks numeriskt.
        /// </summary>
        public string? TextValue { get; init; }

        /// <summary>
        /// Identifierar måttenheten när resultatet har en sådan.
        /// </summary>
        public int? UnitOfMeasureId { get; init; }

        /// <summary>
        /// Anger measurement basis.
        /// </summary>
        public MeasurementBasis Basis { get; init; }

        /// <summary>
        /// Anger detection limit när den rapporterats.
        /// </summary>
        public decimal? DetectionLimit { get; init; }

        /// <summary>
        /// Anger analysmetod när den är känd.
        /// </summary>
        public string? AnalysisMethod { get; init; }

        /// <summary>
        /// Anger resultatets verification state.
        /// </summary>
        public VerificationState VerificationState { get; init; }

        /// <summary>
        /// Innehåller anteckningar om resultatet.
        /// </summary>
        public string? Notes { get; init; }

        /// <summary>
        /// Concurrency-token från databasen.
        /// </summary>
        public uint RowVersion { get; init; }
    }
}
