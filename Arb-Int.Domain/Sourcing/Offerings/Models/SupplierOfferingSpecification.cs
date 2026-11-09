using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class SupplierOfferingSpecification : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid SupplierMaterialOfferingId { get; set; }
        public SupplierOfferingSpecificationKind Kind { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid? ConstituentId { get; set; }
        public Guid? CharacteristicId { get; set; }
        public RequirementOperator Operator { get; set; }
        public decimal? MinimumValue { get; set; }
        public decimal? MaximumValue { get; set; }
        public decimal? TargetValue { get; set; }
        public string? TextValue { get; set; }
        public int? UnitOfMeasureId { get; set; }
        public MeasurementBasis Basis { get; set; }
        public string? AnalysisMethod { get; set; }
        public VerificationState VerificationState { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public Guid? SupplyDocumentId { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplierMaterialOffering SupplierMaterialOffering { get; set; } = null!;
        public Constituent? Constituent { get; set; }
        public Characteristic? Characteristic { get; set; }
        public UnitOfMeasure? UnitOfMeasure { get; set; }
        public SupplyDocument? SupplyDocument { get; set; }
    }
}
