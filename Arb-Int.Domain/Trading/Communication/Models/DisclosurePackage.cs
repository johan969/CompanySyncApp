using Arb_Int.Domain.Base;
using Arb_Int.Domain.Crm;

namespace Arb_Int.Domain.Trading
{
    /// <summary>
    /// Använder paketet för en modererad och anonymiserad delning till exakt en extern affärsrelation.
    /// </summary>
    public class DisclosurePackage : IEntity<Guid>, INamed, IDescribable, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public Guid Id { get; set; }
        public Guid AudienceBusinessRelationshipId { get; set; }
        public DisclosureDirection Direction { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public DisclosurePackageStatus Status { get; set; }
        public string? SourceEntityType { get; set; }
        public Guid? SourceEntityId { get; set; }
        public int LatestRevisionNumber { get; set; }
        public Guid? OwnerUserId { get; set; }
        public string? InternalNotes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public BusinessRelationship AudienceBusinessRelationship { get; set; } = null!;
        public ICollection<DisclosurePackageRevision> Revisions { get; set; } = [];
    }
}
