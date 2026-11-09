using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplyAnalysisResult : IEntity<Guid>, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplyAnalysisId { get; set; }
        public Guid? ConstituentId { get; set; }
        public Guid? CharacteristicId { get; set; }
        public string? AnalyteName { get; set; }
        public AnalysisResultQualifier Qualifier { get; set; }
        public decimal? NumericValue { get; set; }
        public decimal? RangeMaximumValue { get; set; }
        public string? TextValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public MeasurementBasis Basis { get; set; }
        public decimal? DetectionLimit { get; set; }
        public string? AnalysisMethod { get; set; }
        public VerificationState VerificationState { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplyAnalysis SupplyAnalysis { get; set; } = null!;
        public Constituent? Constituent { get; set; }
        public Characteristic? Characteristic { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
    }
}
