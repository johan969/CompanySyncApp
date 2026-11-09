using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Sourcing
{
    public class CertificationScheme : NamedReferenceEntity, IEntity<int>, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public int Id { get; set; }
        public CertificationSchemeKind Kind { get; set; }
        public string? StandardOwner { get; set; }
        public string? StandardVersion { get; set; }
        public string? PublicUrl { get; set; }
        public DateTimeOffset? ValidFromUtc { get; set; }
        public DateTimeOffset? ValidToUtc { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public ICollection<CertificationSchemeJurisdiction> Jurisdictions { get; set; } = [];
    }
}
