namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Innehåller redigerbara värden och target-mapping för ett supply analysis result.
    /// </summary>
    public sealed class UpdateSupplyAnalysisResultRequest
    {
        /// <summary>
        /// Identifierar canonical constituent när resultatet är mappat till en constituent.
        /// </summary>
        public Guid? ConstituentId { get; init; }

        /// <summary>
        /// Identifierar canonical characteristic när resultatet är mappat till en characteristic.
        /// </summary>
        public Guid? CharacteristicId { get; init; }

        /// <summary>
        /// Bevarar rått analyte-namn från källan när ett sådant finns.
        /// </summary>
        public string? AnalyteName { get; init; }

        /// <summary>
        /// Anger hur det rapporterade värdet ska tolkas.
        /// </summary>
        public AnalysisResultQualifier Qualifier { get; init; }

        /// <summary>
        /// Anger numeriskt värde när källan rapporterar ett sådant.
        /// </summary>
        public decimal? NumericValue { get; init; }

        /// <summary>
        /// Anger övre gräns när källan rapporterar ett intervall.
        /// </summary>
        public decimal? RangeMaximumValue { get; init; }

        /// <summary>
        /// Anger textvärde när källan rapporterar ett icke-numeriskt resultat.
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
        /// Concurrency-token för resultatraden.
        /// </summary>
        public uint RowVersion { get; init; }

        /// <summary>
        /// Concurrency-token för supply analysis-aggregatet.
        /// </summary>
        public uint SupplyAnalysisRowVersion { get; init; }
    }
}
