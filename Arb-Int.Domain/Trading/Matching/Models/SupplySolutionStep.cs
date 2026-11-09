using Arb_Int.Domain.Articles;
using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;
using Arb_Int.Domain.Sourcing;

namespace Arb_Int.Domain.Trading
{
    public class SupplySolutionStep : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion
    {
        public Guid Id { get; set; }
        public Guid SupplySolutionCandidateId { get; set; }
        public Guid? SupplySolutionLineId { get; set; }
        public int SequenceNumber { get; set; }
        public SupplySolutionStepType Type { get; set; }
        public SupplySolutionStepStatus Status { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid? ProviderBusinessRelationshipId { get; set; }
        public Guid? SupplierProcessCapabilityId { get; set; }
        public int? ProcessTypeId { get; set; }
        public Guid? InputMaterialId { get; set; }
        public Guid? OutputMaterialId { get; set; }
        public decimal? ExpectedInputQuantity { get; set; }
        public int? InputQuantityUnitId { get; set; }
        public decimal? ExpectedOutputQuantity { get; set; }
        public int? OutputQuantityUnitId { get; set; }
        public decimal? ExpectedYieldPercent { get; set; }
        public int? EstimatedLeadTimeDays { get; set; }
        public Guid? FromLocationId { get; set; }
        public Guid? ToLocationId { get; set; }
        public DateTimeOffset? PlannedStartUtc { get; set; }
        public DateTimeOffset? PlannedEndUtc { get; set; }
        public string? ParametersJson { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public SupplySolutionCandidate SupplySolutionCandidate { get; set; } = null!;
        public SupplySolutionLine? SupplySolutionLine { get; set; }
        public BusinessRelationship? ProviderBusinessRelationship { get; set; }
        public SupplierProcessCapability? SupplierProcessCapability { get; set; }
        public ProcessType? ProcessType { get; set; }
        public Material? InputMaterial { get; set; }
        public Material? OutputMaterial { get; set; }
        public UnitOfMeasure? InputQuantityUnit { get; set; }
        public UnitOfMeasure? OutputQuantityUnit { get; set; }
        public GeographicLocation? FromLocation { get; set; }
        public GeographicLocation? ToLocation { get; set; }
        public ICollection<SupplySolutionStepDependency> PredecessorDependencies { get; set; } = [];
        public ICollection<SupplySolutionStepDependency> SuccessorDependencies { get; set; } = [];
    }
}
