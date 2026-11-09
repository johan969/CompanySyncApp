using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Crm
{
    public class BusinessRelationshipStageHistory : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid BusinessRelationshipId { get; set; }

        public BusinessRelationship BusinessRelationship { get; set; } = null!;

        public int? FromStageId { get; set; }
        public BusinessRelationshipStage? FromStage { get; set; }

        public int ToStageId { get; set; }
        public BusinessRelationshipStage ToStage { get; set; } = null!;

        public DateTimeOffset ChangedUtc { get; set; }

        public Guid? ChangedByUserId { get; set; }

        public string? Notes { get; set; }
    }
}