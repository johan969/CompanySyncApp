using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class BuyerRfqConstituentRequirement : BuyerRfqRequirement
    {
        public Guid ConstituentId { get; set; }
        public RequirementOperator Operator { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
        public decimal? TargetValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public MeasurementBasis Basis { get; set; }
        public string? AnalysisMethod { get; set; }
        public bool MustBeVerifiedForEveryLot { get; set; }
        public bool AccreditedLaboratoryRequired { get; set; }
        public Constituent Constituent { get; set; } = null!;
        public UnitOfMeasure? UnitOfMeasure { get; set; }
    }
}
