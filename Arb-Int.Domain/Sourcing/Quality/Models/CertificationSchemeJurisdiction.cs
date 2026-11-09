using Arb_Int.Domain.Base;
using Arb_Int.Domain.Location;

namespace Arb_Int.Domain.Sourcing
{
    /// <summary>
    /// Använder en separat jurisdiktion eftersom samma certifieringsområde kan gälla ett land, EU eller en annan marknad.
    /// </summary>
    public class CertificationSchemeJurisdiction : IEntity<int>, INamed, ICode, ICreateTracker, IActionTracker, IRowVersion, ISystemVersioned
    {
        public int Id { get; set; }
        public int CertificationSchemeId { get; set; }
        public CertificationJurisdictionKind Kind { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Guid? CountryId { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset? CreateDateUtc { get; set; }
        public string? ActionName { get; set; }
        public DateTimeOffset? ActionDateUtc { get; set; }
        public Guid? ActionUserId { get; set; }
        public uint RowVersion { get; set; }
        public CertificationScheme CertificationScheme { get; set; } = null!;
        public Country? Country { get; set; }
    }
}
