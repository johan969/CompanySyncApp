using Arb_Int.Domain.Base;

namespace Arb_Int.Domain.Parties
{
    public class PartyExternalReference : IEntity<Guid>
    {
        public Guid Id { get; set; }

        public Guid PartyId { get; set; }
        public Party Party { get; set; } = null!;

        /// <summary>
        /// Exempel:<br />
        /// legacy-mariadb<br />
        /// supplier-directory<br />
        /// company-website<br />
        /// manual
        /// </summary>
        public string SourceSystem { get; set; } = null!;

        /// <summary>
        /// Stabil identifierare inom källsystemet.
        /// </summary>
        public string SourceRecordKey { get; set; } = null!;

        public string? SourceUrl { get; set; }

        public DateTimeOffset FirstSeenUtc { get; set; }

        public DateTimeOffset LastSeenUtc { get; set; }

        public string? ContentHash { get; set; }
    }
}